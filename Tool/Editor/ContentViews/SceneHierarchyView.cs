using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using System.Linq;

using WeifenLuo.WinFormsUI.Docking;
using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;

using Otter.UI;
using Otter.Commands;
using Otter.Editor.Commands.ControlCommands;
using Otter.Editor.Commands.ViewCommands;
using Otter.Containers;
using Otter.Interface;

namespace Otter.Editor.ContentViews
{
    /// <summary>
    /// Displays a scene hierarchy
    /// </summary>
    public partial class SceneHierarchyView : DockContent
    {
        #region Events and Delegates
        public delegate void HierarchyEventHandler(object sender);
        public delegate void SceneEventHandler(object sender, GUIScene scene);
        public delegate void ViewEventHandler(object sender, GUIView view);
        public delegate void ControlEventHandler(object sender, GUIControl control);

        public event SceneEventHandler SceneChanged = null;
        public event ViewEventHandler ViewChanged = null;
        public event HierarchyEventHandler SelectionChanged = null;

        public event ViewEventHandler SelectedViewRenamed = null;
        public event ControlEventHandler SelectedControlRenamed = null;

        public event ViewEventHandler BeforeViewRemove = null;
        public event ControlEventHandler BeforeControlRemove = null;
        #endregion

        #region Data
        private GUIScene mScene = null;
        private GUIView mView = null;
        private NotifyingList<GUIControl> mSelectedControls = new NotifyingList<GUIControl>();

        private CommandManager mCommandManager = null;

        private TreeModel mTreeModel = new TreeModel();
        #endregion

        #region Properties
        /// <summary>
        /// Gets / Sets the displayed scene
        /// </summary>
        public GUIScene Scene
        {
            get { return mScene; }
            set 
            {
                if (mScene != value)
                {
                    if (mScene != null)
                    {
                        mScene.OnViewAdded -= new GUIScene.ViewEventHandler(mScene_OnViewAdded);
                        mScene.OnViewRemoved -= new GUIScene.ViewEventHandler(mScene_OnViewRemoved);
                    }

                    mScene = value;

                    if (mScene != null)
                    {
                        mScene.OnViewAdded += new GUIScene.ViewEventHandler(mScene_OnViewAdded);
                        mScene.OnViewRemoved += new GUIScene.ViewEventHandler(mScene_OnViewRemoved);
                    }

                    RebuildHierarchy();

                    if (SceneChanged != null)
                        SceneChanged(this, mScene);
                }
            }
        }

        /// <summary>
        /// Gets / Sets the scene hierarchy's active command manager.
        /// </summary>
        public CommandManager CommandManager
        {
            get { return mCommandManager; }
            set { mCommandManager = value; }
        }

        /// <summary>                                                                                                                                          
        /// Gets / Sets the selected sceneView
        /// </summary>
        public GUIView View
        {
            get { return mView; }
            set 
            {
                if (mView != value)
                {
                    SelectedControls = new NotifyingList<GUIControl>();
                    mView = value;

                    NotifyViewChanged();
                }
            }
        }

        /// <summary>
        /// Gets / Sets the selected control
        /// </summary>
        public NotifyingList<GUIControl> SelectedControls
        {
            get 
            { 
                return mSelectedControls; 
            }
            set
            {
                NotifyingList<GUIControl> list = value;
                if (list == null)
                    list = new NotifyingList<GUIControl>();

                List<GUIControl> intersection = new List<GUIControl>(mSelectedControls.Intersect(list));

                if (mSelectedControls.Count == list.Count && intersection.Count == list.Count)
                    return;

                mSceneTreeView.ClearSelection();

                mSelectedControls.SuppressEvents = true;
                {
                    mSelectedControls.Clear();
                    mSelectedControls.AddRange(list);

                    if (mSelectedControls.Count > 0 && mSelectedControls[0].ParentView != mView)
                        mView = mSelectedControls[0].ParentView;

                    mSceneTreeView.BeginUpdate();
                    {
                        foreach (GUIControl control in mSelectedControls)
                        {
                            TreeNodeAdv node = FindNodeByTag(control, mSceneTreeView.AllNodes);
                            if (node != null)
                                node.IsSelected = true;
                        }
                    }
                    mSceneTreeView.EndUpdate();
                }
                mSelectedControls.SuppressEvents = false;

                NotifySelectionChanged();
            }
        }
        #endregion

        #region Event Notifiers
        /// <summary>
        /// Notifies that the selected sceneView has been changed
        /// </summary>
        private void NotifyViewChanged()
        {
            if (ViewChanged != null)
                ViewChanged(this, View);
        }

        /// <summary>
        /// Notifies that the selected control has been changed
        /// </summary>
        private void NotifySelectionChanged()
        {
            if (SelectionChanged != null)
                SelectionChanged(this);
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Creates a new sceneView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mCreateViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddView();
        }
        
        /// <summary>
        /// Duplicates the selected view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mDuplicateViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DuplicateView();
        }

        /// <summary>
        /// Occurs after a node has been selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mSceneTreeView_SelectionChanged(object sender, EventArgs e)
        {
            TreeNodeAdv treeNode = mSceneTreeView.SelectedNode;
            SceneNode node = (treeNode != null) ? treeNode.Tag as SceneNode : null;
            if (node == null)
                return;

            if (node.Control is GUIView)
            {
                SelectedControls = new NotifyingList<GUIControl>();
                View = node.Control as GUIView;

                // Reset the selection, as setting SelectedControls to null reset the selection
                mSceneTreeView.SelectedNode = treeNode;
            }
            else
            {
                SelectedControls = new NotifyingList<GUIControl>(mSceneTreeView.SelectedNodes.Select((a) => (a.Tag as SceneNode).Control ));
            }
        }

        /// <summary>
        /// Called when the user has pressed a key
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mSceneTreeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DeleteSelectedItem();
            }
        }

        /// <summary>
        /// User chose to rename a selected item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mRenameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mSceneTreeView.EditCurrentNode();
        }
        
        /// <summary>
        /// Called after a label has been edited.  We need to do some name validation here
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void textBox_LabelChanged(object sender, LabelEventArgs e)
        {
            SceneNode node = e.Subject as SceneNode;
            if (node == null)
                return;

            GUIView view = node.Tag as GUIView;
            GUIControl control = node.Tag as GUIControl;
            if(view != null)
            {
                GUIView existingView = Scene.GetView(e.NewLabel);
                if(existingView != null)
                {
                    MessageBox.Show("Name already in use", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // If we got here, it's a unique name.  Change it now.
                view.Name = e.NewLabel;
              
                if (SelectedViewRenamed != null)
                    SelectedViewRenamed(this, view);
            }
            else if(control != null)
            {
                // Check to see if the parent sceneView contains any other controls by the same name.
                if (control.ParentView.Controls.GetControl(e.NewLabel) != null)
                {
                    MessageBox.Show("Name already in use", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // If we got here, it's a unique name.  Change it now.
                control.Name = e.NewLabel;

                if (SelectedControlRenamed != null)
                    SelectedControlRenamed(this, control);
            }
        }

        /// <summary>
        /// User chose to delete a selected item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mDeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteSelectedItem();
        }

        /// <summary>
        /// Called when the context menu strip has opened
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            mCreateViewToolStripMenuItem.Enabled = (mScene != null);
            mDuplicateViewToolStripMenuItem.Enabled = (SelectedControls.Count == 0) && (mView != null);
            mRenameToolStripMenuItem.Enabled = (mSceneTreeView.SelectedNode != null);
            mDeleteToolStripMenuItem.Enabled = (mSceneTreeView.SelectedNode != null);
        }
        #endregion

        #region Event Handlers : GUIScene
        /// <summary>
        /// Called when a sceneView has been added to the scene
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="sceneView"></param>
        void mScene_OnViewAdded(object sender, GUIView view)
        {
            if (view == null)
                return;

            GUIScene scene = sender as GUIScene;
            AddViewNode(view, scene.Views.IndexOf(view));
        }

        /// <summary>
        /// Called when a sceneView has been removed from a scene
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="sceneView"></param>
        void mScene_OnViewRemoved(object sender, GUIView view)
        {
            if (view == null)
                return;

            TreeNodeAdv node = FindNodeByTag(view, mSceneTreeView.AllNodes);
            if (node != null)
            {
                Node modelNode = node.Tag as Node;
                modelNode.Parent = null;
            }
        }
        #endregion

        #region Event Handlers : GUIView
        void Controls_OnControlRemoved(object sender, GUIControl control)
        {
            TreeNodeAdv controlNode = FindNodeByTag(control, mSceneTreeView.AllNodes);
            if (controlNode != null)
            {
                Node modelNode = controlNode.Tag as Node;
                modelNode.Parent = null;
            }
        }

        void Controls_OnControlAdded(object sender, GUIControl control)
        {
            GUIView view = sender as GUIView;
            TreeNodeAdv parentNode = FindNodeByTag(control.Parent, mSceneTreeView.AllNodes);
            if (parentNode != null)
            {
                int index = control.Parent.Controls.IndexOf(control);
                AddControlNode(parentNode.Tag as Node, control, index);
            }
        }
        #endregion

        #region Event Handlers : TreeModel
        /// <summary>
        /// Called when new nodes were inserted into the tree model.
        /// We need to set the nodes' event handlers accordingly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mTreeModel_NodesInserted(object sender, TreeModelEventArgs e)
        {
            foreach (Node node in e.Children)
            {
                SetHandlers(node, true);
            }
        }

        /// <summary>
        /// Nodes have been removed.  Remove the event handlers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mTreeModel_NodesRemoved(object sender, TreeModelEventArgs e)
        {
            foreach (Node node in e.Children)
            {
                SetHandlers(node, false);
            }
        }

        /// <summary>
        /// Helper function to set/unset event handlers
        /// </summary>
        /// <param name="node"></param>
        /// <param name="set"></param>
        void SetHandlers(Node node, bool set)
        {
            SceneNode sceneNode = node as SceneNode;
            if (sceneNode == null)
                return;

            if (set)
            {
                sceneNode.Control.Controls.OnControlAdded += new GUIControlCollection.ControlEventHandler(Controls_OnControlAdded);
                sceneNode.Control.Controls.OnControlRemoved += new GUIControlCollection.ControlEventHandler(Controls_OnControlRemoved);
            }
            else
            {
                sceneNode.Control.Controls.OnControlAdded -= new GUIControlCollection.ControlEventHandler(Controls_OnControlAdded);
                sceneNode.Control.Controls.OnControlRemoved -= new GUIControlCollection.ControlEventHandler(Controls_OnControlRemoved);
            }

            foreach (Node childNode in sceneNode.Nodes)
                SetHandlers(childNode, set);
        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public SceneHierarchyView()
        {
            InitializeComponent();

            this.HideOnClose = true;
            mSceneTreeView.Model = mTreeModel;

            NodeTextBox textBox = new NodeTextBox();
            textBox.DataPropertyName = "Text";
            textBox.Font = this.Font;
            textBox.EditEnabled = true;
            textBox.LabelChanged += new EventHandler<LabelEventArgs>(textBox_LabelChanged);

            mSceneTreeView.NodeControls.Add(textBox);

            mSelectedControls.OnItemAdded += new NotifyingList<GUIControl>.ListEventHandler(mSelectedControls_OnItemAdded);
            mSelectedControls.OnItemRemoved += new NotifyingList<GUIControl>.ListEventHandler(mSelectedControls_OnItemRemoved);

            mTreeModel.NodesInserted += new EventHandler<TreeModelEventArgs>(mTreeModel_NodesInserted);
            mTreeModel.NodesRemoved += new EventHandler<TreeModelEventArgs>(mTreeModel_NodesRemoved);
        }

        void mSelectedControls_OnItemAdded(object sender, GUIControl item)
        {
            TreeNodeAdv node = FindNodeByTag(item, mSceneTreeView.AllNodes);
            if (node != null)
                node.IsSelected = true;

            NotifySelectionChanged();
        }

        void mSelectedControls_OnItemRemoved(object sender, GUIControl item)
        {
            TreeNodeAdv node = FindNodeByTag(item, mSceneTreeView.AllNodes);
            if (node != null)
                node.IsSelected = false;

            NotifySelectionChanged();
        }

        /// <summary>
        /// Adds a new sceneView to the scene
        /// </summary>
        public void AddView()
        {
            string name = "New View ";
            int cnt = 1;
            while (mScene.GetView(name + cnt) != null)
                cnt++;

            GUIView view = new GUIView(name + cnt);
            mCommandManager.AddCommand(new AddViewCommand(mScene, view), true);

            View = view;
        }

        /// <summary>
        /// Duplicates the selected view
        /// </summary>
        public void DuplicateView()
        {
            GUIView view = (GUIView)mView.Clone();
            view.Name = "Copy of" + view.Name;

            mCommandManager.AddCommand(new AddViewCommand(mScene, view), true);
        }

        /// <summary>
        /// Changes to a different view
        /// </summary>
        public void ChangeSelectedView(GUIView view)
        {
            if (view == null)
                mSceneTreeView.SelectedNode = null;
            else
                mSceneTreeView.SelectedNode = FindNodeByTag(view, mSceneTreeView.AllNodes);
        }

        /// <summary>
        /// Repopulates the entire scene hierarchy sceneView.
        /// </summary>
        public void RebuildHierarchy()
        {
            mTreeModel.Nodes.Clear();
            View = null;

            if(mScene == null)
                return;

            foreach (GUIView view in mScene.Views)
            {
                AddViewNode(view);
            }

            mSceneTreeView.ExpandAll();
        }

        /// <summary>
        /// Retrieves a tree node by tag
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        private TreeNodeAdv FindNodeByTag(object tag, IEnumerable<TreeNodeAdv> collection)
        {
            if (collection == null)
                return null;

            foreach (TreeNodeAdv node in collection)
            {
                Node modelNode = (Node)node.Tag;

                if (modelNode.Tag == tag)
                    return node;

                TreeNodeAdv childNode = FindNodeByTag(tag, node.Children);
                if (childNode != null)
                    return childNode;
            }

            return null;
        }

        /// <summary>
        /// Adds a sceneView node to the scene hierarchy
        /// </summary>
        /// <param name="sceneView"></param>
        /// <returns></returns>
        private void AddViewNode(GUIView view)
        {
            AddViewNode(view, -1);
        }
        
        /// <summary>
        /// Adds a sceneView node to the scene hierarchy at a specific index
        /// </summary>
        /// <param name="sceneView"></param>
        /// <returns></returns>
        private void AddViewNode(GUIView view, int atIndex)
        {
            SceneNode node = new SceneNode(view);

            if (atIndex >= 0)
                mTreeModel.Nodes.Insert(atIndex, node);
            else
                mTreeModel.Nodes.Add(node);

            AddControlNodes(node, view.Controls);
        }

        /// <summary>
        /// Adds the GUI Control nodes to the treeview
        /// </summary>
        /// <param name="ancestor"></param>
        /// <param name="sceneView"></param>
        private void AddControlNodes(Node parentNode, GUIControlCollection controls)
        {
            foreach (GUIControl control in controls)
            {
                AddControlNode(parentNode, control);
            }
        }

        /// <summary>
        /// Adds the control to the tree
        /// </summary>
        /// <param name="ancestor"></param>
        /// <param name="control"></param>
        private void AddControlNode(Node parentNode, GUIControl control)
        {
            AddControlNode(parentNode, control, -1);
        }

        /// <summary>
        /// Adds the control to the tree
        /// </summary>
        /// <param name="ancestor"></param>
        /// <param name="control"></param>
        private void AddControlNode(Node parentNode, GUIControl control, int atIndex)
        {
            SceneNode node = new SceneNode(control);

            if (atIndex >= 0)
                 parentNode.Nodes.Insert(atIndex, node);
            else
                parentNode.Nodes.Add(node);

            foreach (GUIControl childControl in control.Controls)
            {
                AddControlNode(node, childControl);
            }
        }

        /// <summary>
        /// Deletes the currently selected item
        /// </summary>
        private void DeleteSelectedItem()
        {
            List<TreeNodeAdv> nodesToRemove = new List<TreeNodeAdv>(mSceneTreeView.SelectedNodes);
            if (nodesToRemove.Count == 0)
                return;

            if (MessageBox.Show("Are you sure you want to delete these items?", "Warning", MessageBoxButtons.YesNo) == DialogResult.No)
                return;

            foreach (TreeNodeAdv node in nodesToRemove)
            {
                GUIControl control = (node.Tag as SceneNode).Control;
                GUIView view = control as GUIView;

                if (view != null)
                {
                    if (BeforeViewRemove != null)
                        BeforeViewRemove(this, view);

                    CommandManager.AddCommand(new DeleteViewCommand(mScene, view), true);

                    View = null;
                }
                else if (control != null)
                {
                    if (BeforeControlRemove != null)
                        BeforeControlRemove(this, control);

                    CommandManager.AddCommand(new DeleteControlCommand(control.ParentView, control), true);

                    SelectedControls = new NotifyingList<GUIControl>();
                }

                (node.Tag as SceneNode).Parent = null;
            }
        }

        private void mSceneTreeView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            mSceneTreeView.DoDragDropSelectedNodes(DragDropEffects.Move);
        }

        private void mSceneTreeView_DragDrop(object sender, DragEventArgs e)
        {
            mSceneTreeView.BeginUpdate();
            {
                TreeNodeAdv[] nodes = (TreeNodeAdv[])e.Data.GetData(typeof(TreeNodeAdv[]));
                Node dropNode = mSceneTreeView.DropPosition.Node.Tag as Node;

                GUIControl parentControl = null;
                List<GUIControl> controls = new List<GUIControl>();
                int index = -1;
                bool bParentChange = false;

                // If dropped inside, we're re-grouping (re-parenting) the controls
                if (mSceneTreeView.DropPosition.Position == NodePosition.Inside)
                {
                    bParentChange = true;

                    SceneNode parent = dropNode as SceneNode;
                    parentControl = parent.Control;

                    // Create a list of controls that we'll re-parent
                    foreach (TreeNodeAdv n in nodes)
                    {
                        SceneNode sceneNode = n.Tag as SceneNode;
                        if (sceneNode.Control is GUIView)
                            continue;

                        controls.Add(sceneNode.Control);
                    }
                }
                else
                {
                    SceneNode parent = dropNode.Parent as SceneNode;
                    Node nextItem = dropNode;
                    if (mSceneTreeView.DropPosition.Position == NodePosition.After)
                        nextItem = dropNode.NextNode;

                    parentControl = (parent != null) ? parent.Control : null;
                    index = dropNode.Parent.Nodes.IndexOf(nextItem);

                    // Create a list of controls we're going to reorder
                    foreach (TreeNodeAdv n in nodes)
                    {
                        SceneNode sceneNode = n.Tag as SceneNode;
                        controls.Add(sceneNode.Control);

                        if (sceneNode.Control.Parent != parentControl)
                            bParentChange = true;
                    }
                }

                if (controls.Count > 0)
                {
                    controls.Sort
                   (
                        (a, b)=>
                        ( 
                            (b is GUIView ? b.Scene.Views.IndexOf((GUIView)b) : b.Parent.Controls.IndexOf(b)) - 
                            (a is GUIView ? a.Scene.Views.IndexOf((GUIView)a) : a.Parent.Controls.IndexOf(a))
                            
                        )
                    );

                    if(bParentChange)
                        mCommandManager.AddCommand(new ChangeParentCommand(controls, parentControl, index), true);
                    else
                        mCommandManager.AddCommand(new ChangeOrderCommand(controls, index), true);
                }
                
                mSceneTreeView.DropPosition.Node.IsExpanded = true;
            }
            mSceneTreeView.EndUpdate();
        }

        private void mSceneTreeView_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TreeNodeAdv[])) && mSceneTreeView.DropPosition.Node != null)
            {
                TreeNodeAdv[] nodes = e.Data.GetData(typeof(TreeNodeAdv[])) as TreeNodeAdv[];
                TreeNodeAdv parent = mSceneTreeView.DropPosition.Node;
                if (mSceneTreeView.DropPosition.Position != NodePosition.Inside)
                    parent = parent.Parent;

                SceneNode parentSceneNode = parent.Tag as SceneNode;
                bool bParentIsView = parentSceneNode != null && parentSceneNode.Control is GUIView;

                foreach (TreeNodeAdv node in nodes)
                {
                    SceneNode sceneNode = node.Tag as SceneNode;

                    // Can't re-parent to one of its children
                    if (!CheckNodeParent(parent, node))
                    {
                        e.Effect = DragDropEffects.None;
                        return;
                    }

                    // GUIViews can only be direct children of the root (scene)
                    if (sceneNode.Control is GUIView && (parent != mSceneTreeView.Root))
                    {
                        e.Effect = DragDropEffects.None;
                        return;
                    }

                    if (!(sceneNode.Control is GUIView))
                    {
                        GUIView parentView = sceneNode.Control.ParentView;
                        GUIView targetParentView = null;
                        
                        if(parentSceneNode != null)
                        {
                            if(bParentIsView)
                                targetParentView = parentSceneNode.Control as GUIView;
                            else
                                targetParentView =  parentSceneNode.Control.ParentView;
                        }

                        // Can't reparent to a different view
                        if (parentView != targetParentView)
                        {
                            e.Effect = DragDropEffects.None;
                            return;
                        }

                        // Can't reparent to a non-GUIGroup
                        if (!(parentSceneNode.Control is GUIGroup) && !(parentSceneNode.Control is GUIView))
                        {
                            e.Effect = DragDropEffects.None;
                            return;
                        }
                    }
                }

                e.Effect = e.AllowedEffect;
            }
        }

        private bool CheckNodeParent(TreeNodeAdv parent, TreeNodeAdv node)
        {
            while (parent != null)
            {
                if (node == parent)
                    return false;
                else
                    parent = parent.Parent;
            }
            return true;
        }
    }

    class SceneNode : Node
    {
        private GUIControl mControl = null;
        public GUIControl Control
        {
            get { return mControl; }
            set { mControl = value; }
        }

        public override string Text
        {
            get
            {
                if (mControl.Locked && mControl.Hidden)
                    return mControl.Name + " [L,H]";
                else if (mControl.Locked)
                    return mControl.Name + " [L]";
                else if (mControl.Hidden)
                    return mControl.Name + " [H]";
                else
                    return mControl.Name;
            }
            set
            {
            }
        }

        public SceneNode(GUIControl control)
            : base(control.Name)
        {
            mControl = control;
            this.Tag = control;
        }
    }
}
