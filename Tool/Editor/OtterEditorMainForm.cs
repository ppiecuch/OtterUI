using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Threading;
using Otter.Editor.Commands;
using Otter.Editor.ContentViews;
using Otter.Editor.Forms;
using Otter.Export;
using Otter.Forms;
using Otter.Plugins;
using Otter.Project;
using Otter.UI;

using Otter.Containers;
using WeifenLuo.WinFormsUI.Docking;

/* 
 * TODO : Move all the event handlers in separate regions
 */
namespace Otter.Editor
{
    public partial class OtterEditorMainForm : Form
    {
        #region Data
        private WelcomeForm mWelcomeForm = null;
        private DeserializeDockContent mDeserializeDockContent = null;

        private String mLayoutPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/Aonyx/Otter/layout.xml";
        private string mDefaultProject = "";
        private bool mIgnoreSelectionChanges = false;

        private bool mCancelAppQuit = false;
        #endregion

        #region Event Handlers
        /// <summary>
        /// Called when the Otter.Editor main sceneView loads.  Sets up
        /// the docking views, loads previous data, etc.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OtterEditorMainForm_Load(object sender, EventArgs e)
        {
            mOnlineHelpToolStripMenuItem.ToolTipText = Otter.Editor.Properties.Settings.Default.HelpURL;

            SetupPlugins();

            Otter.Interface.Graphics.Instance = new Otter.Interface.Graphics(this.Handle.ToInt32());
            Otter.Interface.Graphics.Instance.OnTextureLoaded += new Otter.Interface.Graphics.TextureEventHandler(Graphics_OnTextureLoaded);
            Otter.Interface.Graphics.Instance.OnTextureUnloaded += new Otter.Interface.Graphics.TextureEventHandler(Graphics_OnTextureUnloaded);

            bool bLoaded = false;
            try
            {
                if (!File.Exists(mLayoutPath))
                {
                    string name = "Otter.Editor.res.default_layout.xml";
                    Stream stream = null;
                    try
                    {
                        Assembly assembly = Assembly.GetExecutingAssembly();
                        stream = assembly.GetManifestResourceStream(name);

                        mDockPanel.LoadFromXml(stream, mDeserializeDockContent);
                        bLoaded = true;
                    }
                    catch(Exception ex)
                    {
                        System.Console.WriteLine("Failed to load resource " + name + ".  Reason: " + ex.Message);
                    }
                    finally
                    {
                        if (stream != null)
                            stream.Close();
                    }
                }
                else if (File.Exists(mLayoutPath))
                {
                    mDockPanel.LoadFromXml(mLayoutPath, mDeserializeDockContent);
                    bLoaded = true;
                }
            }
            catch(Exception)
            {
                Globals.ControlsView.Hide();
                Globals.HistoryView.Hide();
                Globals.ProjectView.Hide();
                Globals.SceneHierarchyView.Hide();
                Globals.PropertiesView.Hide();
                Globals.TimelineView.Hide();
            }

            if(!bLoaded)
            {
                // Add the different forms to the panel.
                Globals.ControlsView.Show(mDockPanel, DockState.DockRightAutoHide);
                Globals.HistoryView.Show(mDockPanel, DockState.DockRightAutoHide);

                Globals.ProjectView.Show(mDockPanel, DockState.DockLeft);
                Globals.SceneHierarchyView.Show(Globals.ProjectView.Pane, DockAlignment.Bottom, 0.5f);
                Globals.PropertiesView.Show(mDockPanel, DockState.DockRight);
                Globals.TimelineView.Show(mDockPanel, DockState.DockBottom);

                Globals.SceneHierarchyView.Show();
            }

            Globals.PropertiesView.DockStateChanged += new EventHandler(DockContent_DockStateChanged);
            Globals.ProjectView.DockStateChanged += new EventHandler(DockContent_DockStateChanged);
            Globals.TimelineView.DockStateChanged += new EventHandler(DockContent_DockStateChanged);
            Globals.ControlsView.DockStateChanged += new EventHandler(DockContent_DockStateChanged);
            Globals.SceneHierarchyView.DockStateChanged += new EventHandler(DockContent_DockStateChanged);
            Globals.HistoryView.DockStateChanged += new EventHandler(DockContent_DockStateChanged);

            // Set up the dock states.
            UpdateViewMenu();
            
            // Now add the checked-event handler
            projectViewToolStripMenuItem.CheckedChanged += new System.EventHandler(this.viewToolStripMenuItem_CheckedChanged);
            propertiesViewToolStripMenuItem.CheckedChanged += new System.EventHandler(this.viewToolStripMenuItem_CheckedChanged);
            timelineViewToolStripMenuItem.CheckedChanged += new System.EventHandler(this.viewToolStripMenuItem_CheckedChanged);
            controlsViewToolStripMenuItem.CheckedChanged += new System.EventHandler(this.viewToolStripMenuItem_CheckedChanged);
            sceneHierarchyViewToolStripMenuItem.CheckedChanged += new EventHandler(this.viewToolStripMenuItem_CheckedChanged);
            historyViewToolStripMenuItem.CheckedChanged += new EventHandler(this.viewToolStripMenuItem_CheckedChanged);

            // ControlsView events
            Globals.ControlsView.OnControlSelectionChanged += new ControlsView.ControlsViewDelegate(ControlsView_OnControlSelectionChanged);

            // PropertiesView Events
            Globals.PropertiesView.PropertyValueChanged += new PropertyValueChangedEventHandler(PropertiesView_PropertyValueChanged);

            // Project View Events
            Globals.ProjectView.OnCreateEntry += new ProjectView.EntryEventHandler(ProjectView_OnCreateEntry);
            Globals.ProjectView.OnDeleteEntry += new ProjectView.EntryEventHandler(ProjectView_OnDeleteEntry);
            Globals.ProjectView.OnRenameEntry += new ProjectView.EntryEventHandler(ProjectView_OnRenameEntry);
            Globals.ProjectView.OnOpenEntry += new ProjectView.EntryEventHandler(ProjectView_OnOpenEntry);
            Globals.ProjectView.OnClosingEntry += new ProjectView.EntryActionEventHandler(ProjectView_OnClosingEntry);
            Globals.ProjectView.OnCloseEntry += new ProjectView.EntryEventHandler(ProjectView_OnCloseEntry);
            Globals.ProjectView.OnExportEntry += new ProjectView.EntryEventHandler(ProjectView_OnExportEntry);

            // Scene Hierarchy View Events
            Globals.SceneHierarchyView.SelectionChanged += new SceneHierarchyView.HierarchyEventHandler(SceneHierarchyView_SelectionChanged);
            Globals.SceneHierarchyView.ViewChanged += new SceneHierarchyView.ViewEventHandler(SceneHierarchyView_ViewChanged);
            Globals.SceneHierarchyView.SelectedViewRenamed += new SceneHierarchyView.ViewEventHandler(SceneHierarchyView_SelectedViewRenamed);
            Globals.SceneHierarchyView.SelectedControlRenamed += new SceneHierarchyView.ControlEventHandler(SceneHierarchyView_SelectedControlRenamed);

            // Timeline View Events
            Globals.TimelineView.AnimationUpdated += new TimelineView.TimelineViewEventHandler(TimelineView_AnimationUpdated);
            Globals.TimelineView.SelectedControlChanged += new TimelineView.TimelineViewEventHandler(TimelineView_SelectedControlChanged);
            Globals.TimelineView.FrameSelectionChanged += new TimelineView.TimelineViewEventHandler(TimelineView_FrameSelectionChanged);

            mDockPanel.ActiveDocumentChanged += new EventHandler(mDockPanel_ActiveDocumentChanged);
            mDockPanel.ContentRemoved += new EventHandler<DockContentEventArgs>(mDockPanel_ContentRemoved);

            PopulateRecentItemsMenu();

            if (mDefaultProject != "")
            {
                if (OpenProject(mDefaultProject))
                    return;
            }

            if (Otter.Editor.Properties.Settings.Default.ShowWelcome)
            {
                mWelcomeForm = new WelcomeForm();

                mWelcomeForm.OnLoadProject += new WelcomeForm.WelcomeActionDelegate(welcomeForm_OnLoadProject);
                mWelcomeForm.OnLoadRecentProject += new WelcomeForm.LoadRecentProjectDelegate(welcomeForm_OnLoadRecentProject);
                mWelcomeForm.OnNewProject += new WelcomeForm.WelcomeActionDelegate(welcomeForm_OnNewProject);

                mWelcomeForm.Show(this);
            }
        }

        /// <summary>
        /// Creates a new, default project
        /// </summary>
        void welcomeForm_OnNewProject(object sender)
        {
            CreateNewProject();
        }

        /// <summary>
        /// Loads a recent project from file
        /// </summary>
        /// <param name="path"></param>
        void welcomeForm_OnLoadRecentProject(object sender, string path)
        {
            if (!System.IO.File.Exists(path))
            {
                if (MessageBox.Show("Project file could not be found.  Remove from recent projects?", "Project Not Found", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    RemoveProjectFromRecents(path);

                    WelcomeForm welcomeForm = sender as WelcomeForm;
                    welcomeForm.RefreshRecentProjects();
                }

                return;
            }

            OpenProject(path);
        }

        /// <summary>
        /// Browses to and loads a project from file
        /// </summary>
        void welcomeForm_OnLoadProject(object sender)
        {
            OpenProject();
        }

        /// <summary>
        /// Called whenever the check-state of the Project/Timeline/Properties/etc
        /// menu items changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void viewToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            object[] list = new object[]
            {
                new object[]{ propertiesViewToolStripMenuItem, Globals.PropertiesView },
                new object[]{ projectViewToolStripMenuItem, Globals.ProjectView },
                new object[]{ timelineViewToolStripMenuItem, Globals.TimelineView },
                new object[]{ controlsViewToolStripMenuItem, Globals.ControlsView },
                new object[]{ sceneHierarchyViewToolStripMenuItem, Globals.SceneHierarchyView },
                new object[]{ historyViewToolStripMenuItem, Globals.HistoryView }
            };

            for (int i = 0; i < list.Count(); i++)
            {
                object[] subList = list[i] as object[];
                if (subList[0] == sender)
                {
                    ToolStripMenuItem item = subList[0] as ToolStripMenuItem;

                    if (item.Checked)
                        ((DockContent)subList[1]).Show();
                    else
                        ((DockContent)subList[1]).Hide();
                }
            }
        }

        /// <summary>
        /// Called when the main sceneView is closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OtterEditorMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            bool bPromptSave = Globals.ProjectView.ProjectModified;
            IDockContent[] docArray = mDockPanel.Documents.ToArray();

            if (mCancelAppQuit)
            {
                foreach (DockContent doc in docArray)
                {
                    SceneView sceneView = doc as SceneView;
                    if (sceneView != null)
                    {
                        sceneView.PromptSave = true;
                        break;
                    }
                }

                e.Cancel = true;
                return;
            }

            // Project itself did not require saving.  Cycle through the scenes
            // and check to see if they've changed since the last save.
            if (!bPromptSave)
            {
                foreach (DockContent doc in docArray)
                {
                    SceneView sceneView = doc as SceneView;
                    if (sceneView != null && sceneView.Modified && sceneView.PromptSave)
                    {
                        bPromptSave = true;
                        break;
                    }
                }
            }

            if (bPromptSave)
            {
                DialogResult result = MessageBox.Show("You have unsaved changes.\nWould you like to save now before closing?",
                    "Save Changes",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }

                if (result == DialogResult.Yes)
                {
                    if (CanSaveProject(true) && SaveProject())
                    {
                        Globals.ProjectView.ProjectModified = false;
                    }
                    else
                    {
                        e.Cancel = true;
                        return;
                    }
                }
            }

            try
            {
                String dir = Path.GetDirectoryName(mLayoutPath);

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                mDockPanel.SaveAsXml(mLayoutPath);
            }
            catch (Exception ex)
            {
                // Failed for some reason
                System.Console.WriteLine("Exception: " + ex.Message);
            }

            foreach (DockContent doc in docArray)
            {
                SceneView sceneView = doc as SceneView;
                if (sceneView != null)
                    sceneView.PromptSave = false;

                doc.Close();
            }

            if (Otter.Interface.Graphics.Instance != null)
            {
                Otter.Interface.Graphics.Instance.Dispose();
                Otter.Interface.Graphics.Instance = null;
            }
        }
        #endregion

        #region Event Handlers : Dock Contents

        /// <summary>
        /// Called when DockContent's state has changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DockContent_DockStateChanged(object sender, EventArgs e)
        {
            UpdateViewMenu();
        }
        #endregion

        #region Event Handlers : Dock Panel
        /// <summary>
        /// Called when the active document has changed.  Update the panels
        /// accordingly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mDockPanel_ActiveDocumentChanged(object sender, EventArgs e)
        {
            SceneView sceneView = mDockPanel.ActiveDocument as SceneView;

            if (sceneView != null)
            {
                if (sceneView.PrimaryControl != null)
                    Globals.PropertiesView.PropertyGrid.SelectedObject = sceneView.PrimaryControl;
                else
                    Globals.PropertiesView.PropertyGrid.SelectedObject = sceneView.ActiveView;

                Globals.SceneHierarchyView.Scene = sceneView.Scene;
                Globals.SceneHierarchyView.View = sceneView.ActiveView;
                Globals.SceneHierarchyView.SelectedControls = sceneView.SelectedControls;
                Globals.SceneHierarchyView.CommandManager = sceneView.CommandManager;

                Globals.HistoryView.CommandManager = sceneView.CommandManager;
                Globals.TimelineView.View = sceneView.ActiveView;
                Globals.TimelineView.CommandManager = sceneView.CommandManager;

                mSceneToolStripMenuItem.Enabled = true;

                // Set focus to the first active sceneView
                if (sceneView.ActiveView == null)
                    sceneView.ActiveView = sceneView.Scene.Views.Count > 0 ? sceneView.Scene.Views[0] : null;
            }
            else
            {
                Globals.SceneHierarchyView.Scene = null;
                Globals.SceneHierarchyView.View = null;
                Globals.SceneHierarchyView.SelectedControls = null;
                Globals.SceneHierarchyView.CommandManager = null;
                Globals.PropertiesView.PropertyGrid.SelectedObject = null;
                Globals.HistoryView.CommandManager = null;
                Globals.TimelineView.View = null;
                Globals.TimelineView.CommandManager = null;

                mSceneToolStripMenuItem.Enabled = false;
            }            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mDockPanel_ContentRemoved(object sender, DockContentEventArgs e)
        {
            SceneView view = e.Content as SceneView;

            if (view != null)
            {
                foreach (GUIProjectEntry entry in GUIProject.CurrentProject.Entries)
                {
                    if (entry is GUIProjectScene && ((GUIProjectScene)entry).Scene == view.Scene)
                    {
                        entry.Close();
                    }
                }
            }
        }
        #endregion

        #region Event Handlers : Project View
        /// <summary>
        /// Called when an entry has been created
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="entry"></param>
        void ProjectView_OnCreateEntry(object sender, GUIProjectEntry entry)
        {
            GUIProjectScene sceneEntry = entry as GUIProjectScene;
            if (sceneEntry == null)
                return;

            if (sceneEntry.Open())
            {
                sceneEntry.Scene.AddView(new GUIView("Default View"));
            }
        }

        /// <summary>
        /// Called when an entry has been deleted
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="entry"></param>
        void ProjectView_OnDeleteEntry(object sender, GUIProjectEntry entry)
        {
        }

        /// <summary>
        /// Called when an entry has been renamed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="entry"></param>
        void ProjectView_OnRenameEntry(object sender, GUIProjectEntry entry)
        {
            GUIProjectScene sceneEntry = entry as GUIProjectScene;
            if(sceneEntry == null)
                return;

            foreach (SceneView view in mDockPanel.Documents)
            {
                if (view.Scene == sceneEntry.Scene)
                {
                    view.TabText = sceneEntry.Name;
                }
            }
        }

        /// <summary>
        /// Opens a scene for editing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="scene"></param>
        void ProjectView_OnOpenEntry(object sender, GUIProjectEntry entry)
        {
            OpenEntry(entry);
        }

        /// <summary>
        /// Called when an entry is about to be closed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void ProjectView_OnClosingEntry(object sender, ProjectEntryActionEventArgs args)
        {
            // TODO - Check to see if we need to save anything here

            GUIProjectScene projectScene = args.Entry as GUIProjectScene;
            if (projectScene != null && projectScene.Scene != null)
            {
                // Find the view associated with the scene
                foreach (SceneView view in mDockPanel.Documents)
                {
                    if (view.Scene == projectScene.Scene)
                    {
                        view.Close();
                        break;
                    }
                }
            }            
        }

        /// <summary>
        /// Called when an entry has been closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="entry"></param>
        void ProjectView_OnCloseEntry(object sender, GUIProjectEntry entry)
        {
        }

        /// <summary>
        /// Exports an entry
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="entry"></param>
        void ProjectView_OnExportEntry(object sender, GUIProjectEntry entry)
        {
            GUIProjectScene projectScene = entry as GUIProjectScene;
            if (projectScene != null)
            {
                if(projectScene.Open())
                    ExportScene(projectScene);
            }
        }
        #endregion

        #region Event Handlers : Contols View
        /// <summary>
        /// Called when the controls view selection has changed
        /// </summary>
        /// <param name="sender"></param>
        void ControlsView_OnControlSelectionChanged(object sender)
        {
            SceneView sceneView = mDockPanel.ActiveDocument as SceneView;
            if (sceneView != null && Globals.ControlsView.SelectedType != null)
            {
                sceneView.CreateControlType = Globals.ControlsView.SelectedType;
            }
        }
        #endregion

        #region Event Handlers : Properties ParentView
        /// <summary>
        /// Properties ActiveView has had a value changed.  Refresh the active
        /// document
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        void PropertiesView_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            SceneView view = mDockPanel.ActiveDocument as SceneView;
            if (view != null)
            {
                view.RefreshRenderPanel();

                PropertyDescriptor descriptor = e.ChangedItem.PropertyDescriptor;

                object obj = Globals.PropertiesView.PropertyGrid.SelectedObject;                
                if(Globals.PropertiesView.PropertyGrid.SelectedObjects != null && Globals.PropertiesView.PropertyGrid.SelectedObjects.Length > 1)
                    obj = Globals.PropertiesView.PropertyGrid.SelectedObjects;

                if (e.ChangedItem.Parent != null && e.ChangedItem.Parent.Value != null)
                    obj = e.ChangedItem.Parent.Value;

                object newVal = e.ChangedItem.PropertyDescriptor.GetValue(obj);
                object oldVal = e.OldValue;

                view.CommandManager.AddCommand(new PropertyChangedCommand(obj, descriptor, oldVal, newVal), false);

                List<GUIControl> controls = new List<GUIControl>();
                if (obj is GUIControl)
                {
                    controls.Add(obj as GUIControl);
                }
                else if(obj is GUIControl[])
                {
                    controls.AddRange(obj as GUIControl[]);
                }

                foreach(GUIControl control in controls)
                {
                    if (Otter.Editor.Properties.Settings.Default.AutoKeyFrame && control.ParentView != null)
                        control.ParentView.CreateKeyFrame(control);
                }                    

                if (e.ChangedItem.PropertyDescriptor.Name == "Name" || e.ChangedItem.PropertyDescriptor.Name == "Locked" || e.ChangedItem.PropertyDescriptor.Name == "Hidden")
                {
                    Globals.TimelineView.Refresh();
                    Globals.SceneHierarchyView.Refresh();
                }
            }
        }
        #endregion

        #region Event Handlers : Timeline ParentView
        /// <summary>
        /// Called when the timeline sceneView has updated the channel somehow
        /// </summary>
        /// <param name="sender"></param>
        void TimelineView_AnimationUpdated(object sender)
        {
            SceneView sceneView = mDockPanel.ActiveDocument as SceneView;
            if (sceneView != null && sceneView.ActiveView == Globals.TimelineView.View)
                sceneView.RefreshRenderPanel();

            // if (Globals.PropertiesView.PropertyGrid.SelectedObject == Globals.TimelineView.SelectedControls)
                Globals.PropertiesView.RefreshProperties();
        }

        /// <summary>
        /// Called when the selected control has been changed.
        /// </summary>
        /// <param name="sender"></param>
        void TimelineView_SelectedControlChanged(object sender)
        {
            if (mIgnoreSelectionChanges)
                return; 
            
            SelectControls(Globals.TimelineView, Globals.TimelineView.View, Globals.TimelineView.SelectedControls);
        }

        /// <summary>
        /// Called when the frame selection has changed
        /// </summary>
        /// <param name="sender"></param>
        void TimelineView_FrameSelectionChanged(object sender)
        {
            List<Otter.UI.Animation.BaseFrame> selectedFrames = Globals.TimelineView.SelectedKeyFrames;

            if (selectedFrames.Count > 0)
                Globals.PropertiesView.PropertyGrid.SelectedObjects = selectedFrames.ToArray();
            else
                Globals.PropertiesView.PropertyGrid.SelectedObjects = null;
        }
        #endregion

        #region Event Handlers : Scene Hierarchy ParentView
        /// <summary>
        /// Called when the selection in the scene hierarchy has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="sceneView"></param>
        void SceneHierarchyView_SelectionChanged(object sender)
        {
            if (mIgnoreSelectionChanges)
                return;

            SceneView sceneView = mDockPanel.ActiveDocument as SceneView;
            if (sceneView != null && sceneView.Scene == Globals.SceneHierarchyView.Scene)
            {
                sceneView.ActiveView = Globals.SceneHierarchyView.View;

                if(Globals.SceneHierarchyView.SelectedControls.Count > 0)
                    SelectControls(Globals.SceneHierarchyView, Globals.SceneHierarchyView.View, Globals.SceneHierarchyView.SelectedControls);
                else
                    SelectControl(Globals.SceneHierarchyView, Globals.SceneHierarchyView.View, Globals.SceneHierarchyView.View);

                sceneView.PrimaryControl = Globals.SceneHierarchyView.SelectedControls.Count > 0 ? Globals.SceneHierarchyView.SelectedControls[0] : null;
            }
        }

        /// <summary>
        /// Called when the selected sceneView has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="sceneView"></param>
        void SceneHierarchyView_ViewChanged(object sender, GUIView view)
        {
            SceneView sceneView = mDockPanel.ActiveDocument as SceneView;
            if (sceneView != null && sceneView.Scene == Globals.SceneHierarchyView.Scene)
            {
                sceneView.ActiveView = Globals.SceneHierarchyView.View;
                sceneView.PrimaryControl = Globals.SceneHierarchyView.SelectedControls.Count > 0 ? Globals.SceneHierarchyView.SelectedControls[0] : null;
            }
        }

        /// <summary>
        /// Called when the selected sceneView has been renamed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="sceneView"></param>
        void SceneHierarchyView_SelectedViewRenamed(object sender, GUIView view)
        {
        }

        /// <summary>
        /// Called when the selected control has been renamed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="control"></param>
        void SceneHierarchyView_SelectedControlRenamed(object sender, GUIControl control)
        {
            Globals.TimelineView.Refresh();
            Globals.PropertiesView.RefreshProperties();
        }
        #endregion

        #region Event Handlers : Scene View
        /// <summary>
        /// Called when a sceneView's control selection has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="control"></param>
        void SceneView_ControlsSelectionChanged(object sender, List<Otter.UI.GUIControl> controls)
        {
            if (mIgnoreSelectionChanges)
                return;

            SceneView sceneView = sender as SceneView;
            if (sceneView == null)
                return;

            // Make sure this scene sceneView is the active sceneView
            if (mDockPanel.ActiveDocument as SceneView != sceneView)
                return;

            SelectControls(sceneView, sceneView.ActiveView, controls);
        }

        /// <summary>
        /// Called when a scene's active sceneView has changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="sceneView"></param>
        void SceneView_ActiveViewChanged(object sender, GUIView view)
        {
            Globals.PropertiesView.PropertyGrid.SelectedObject = view;
            Globals.SceneHierarchyView.View = view;
            Globals.TimelineView.View = view;
        }

        /// <summary>
        /// Called when a control has been updated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="control"></param>
        void SceneView_ControlUpdated(object sender, GUIControl control)
        {
            if (Globals.PropertiesView.PropertyGrid.SelectedObject == control)
                Globals.PropertiesView.RefreshProperties();
        }

        /// <summary>
        /// Input focus has entered the Scene View.  Set the appropriate
        /// object on the properties window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SceneView_Enter(object sender, EventArgs e)
        {
            SceneView sceneView = sender as SceneView;

            if (sceneView == null)
                return;

            if (sceneView.PrimaryControl != null)
                Globals.PropertiesView.PropertyGrid.SelectedObject = sceneView.PrimaryControl;
            else
                Globals.PropertiesView.PropertyGrid.SelectedObject = sceneView.ActiveView;
        }

        /// <summary>
        /// SceneView is closing.  This will be called first before the main form closes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SceneView_FormClosing(object sender, FormClosingEventArgs e)
        {
            mCancelAppQuit = false;
            SceneView sceneView = sender as SceneView;

            if (sceneView == null)
                return;

            if (sceneView.Modified && sceneView.PromptSave)
            {
                DialogResult result = MessageBox.Show("Your scene has unsaved changes.\nWould you like to save now before closing?",
                       "Save Changes",
                       MessageBoxButtons.YesNoCancel,
                       MessageBoxIcon.Question);

                if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    mCancelAppQuit = true;
                    return;
                }

                if (result == DialogResult.Yes)
                {
                    // Find which SceneEntry this belongs to
                    foreach (GUIProjectEntry entry in GUIProject.CurrentProject.Entries)
                    {
                        // .. and save it
                        if (entry is GUIProjectScene && ((GUIProjectScene)entry).Scene == sceneView.Scene)
                        {
                            entry.Save();
                        }
                    }
                }
                else
                {
                    sceneView.PromptSave = false;
                }
            }
        }

        /// <summary>
        /// The Scene View has closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SceneView_FormClosed(object sender, FormClosedEventArgs e)
        {
            SceneView sceneView = sender as SceneView;

            if (sceneView == null)
                return;

            UnloadResources(sceneView);
        }
        #endregion

        #region Event Handlers : Menu Strip
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Globals.HistoryView.Undo();
            Globals.PropertiesView.RefreshProperties();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Globals.HistoryView.Redo();
            Globals.PropertiesView.RefreshProperties();
        }

        /// <summary>
        /// Closes the editor.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Occurs when the "File" Menu is opening
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fileToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            saveToolStripMenuItem.Enabled = (GUIProject.CurrentProject != null);
        }

        /// <summary>
        /// Called when the user clicked on File->New
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateNewProject();
        }

        /// <summary>
        /// Called when the user clicked on File->Open
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenProject();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void RecentItem_Click(object sender, EventArgs e)
        {
            string path = (sender as ToolStripMenuItem).Tag as string;
            OpenProject(path);
        }

        /// <summary>
        /// Saves the project to disk
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(CanSaveProject(true))
                SaveProject();
        }

        /// <summary>
        /// Brings up the font editor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mFontsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListEditor<GUIFont> fontListEditor = new ListEditor<GUIFont>(GUIProject.CurrentProject.Fonts);
            fontListEditor.AddingItem += new ListEditor<GUIFont>.ListEditorEventHandler(fontListEditor_AddingItem);

            fontListEditor.Text = "Fonts";
            fontListEditor.ShowDialog();

            GUIProject.CurrentProject.Fonts = fontListEditor.List;
            Globals.ProjectView.ProjectModified = true;

            SceneView sceneView = mDockPanel.ActiveDocument as SceneView;
            if (sceneView != null)
                sceneView.RefreshRenderPanel();
        }

        /// <summary>
        /// Brings up the Platforms List Editor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mPlatformsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListEditor<Platform> platformListEditor = new ListEditor<Platform>(GUIProject.CurrentProject.Platforms);
            platformListEditor.AddingItem += new ListEditor<Platform>.ListEditorEventHandler(platformListEditor_AddingItem);

            platformListEditor.Text = "Platforms";
            platformListEditor.ShowDialog();

            GUIProject.CurrentProject.Platforms = platformListEditor.List;
            Globals.ProjectView.ProjectModified = true;

            foreach (SceneView view in mDockPanel.Documents)
            {
                view.SetPlatforms(GUIProject.CurrentProject.Platforms);
            }
        }

        /// <summary>
        /// Brings up the Resolutions List Editor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mResolutionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListEditor<Resolution> resolutionListEditor = new ListEditor<Resolution>(GUIProject.CurrentProject.Resolutions);
            resolutionListEditor.Text = "Resolutions";
            resolutionListEditor.ShowDialog();

            GUIProject.CurrentProject.Resolutions = resolutionListEditor.List;
            Globals.ProjectView.ProjectModified = true;

            foreach (SceneView view in mDockPanel.Documents)
            {
                view.SetResolutions(GUIProject.CurrentProject.Resolutions);
            }
        }

        /// <summary>
        /// Occurs when the "Project" Menu is opening
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void projectToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            mFontsToolStripMenuItem.Enabled = (GUIProject.CurrentProject != null);
            mPlatformsToolStripMenuItem.Enabled = (GUIProject.CurrentProject != null);
        }

        /// <summary>
        /// User clicked on the Preferences Menu Item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mPreferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PreferencesForm preferencesForm = new PreferencesForm();
            if (preferencesForm.ShowDialog() == DialogResult.OK)
            {
                SceneView sceneView = mDockPanel.ActiveDocument as SceneView;
                if (sceneView != null)
                    sceneView.RefreshRenderPanel();
            }
        }

        /// <summary>
        /// Brings up the scene's texture manager
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mTexturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SceneView view = mDockPanel.ActiveDocument as SceneView;
            if (view == null || view.Scene == null)
                return;

            TextureManagerForm textureManagerForm = new TextureManagerForm(view.Scene, false);
            textureManagerForm.ShowDialog();

            SceneView sceneView = mDockPanel.ActiveDocument as SceneView;
            if (sceneView != null)
                sceneView.RefreshRenderPanel();
        }

        /// <summary>
        /// Brings up the scene's sound manager
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mSoundsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SceneView view = mDockPanel.ActiveDocument as SceneView;
            if (view == null || view.Scene == null)
                return;

            SoundManagerForm soundManagerForm = new SoundManagerForm(view.Scene);
            soundManagerForm.ShowDialog();
        }

        /// <summary>
        /// Brings up the Messages List Editor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mMessagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SceneView view = mDockPanel.ActiveDocument as SceneView;
            if (view == null || view.Scene == null)
                return;

            MessagesEditor messagesEditor = new MessagesEditor(view.Scene);
            messagesEditor.ShowDialog();
        }
        
        /// <summary>
        /// Called when the user clicked on one of the plugins
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PluginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (item == null)
                return;

            Type type = item.Tag as Type;
            if (type == null)
                return;

            IPlugin plugin = type.GetConstructor(System.Type.EmptyTypes).Invoke(null) as IPlugin;
            if (plugin == null)
                return;

            // TODO - anything else here?
        }
        #endregion

        #region Event Handlers : List Editor
        /// <summary>
        /// Called when a new item has been created and is about to be entered into the list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void platformListEditor_AddingItem(object sender, ListEditor<Platform>.ListEditorEventArgs e)
        {
            ListEditor<Platform> editor = sender as ListEditor<Platform>;
            Platform platform = e.Object as Platform;

            if (editor != null && platform != null)
            {
                // This is a new platform.  Give it a unique name
                int cnt = 1;
                foreach (Platform existingPlatform in editor.List)
                {
                    if (existingPlatform.Name == ("Platform " + cnt))
                        cnt++;
                }

                platform.Name = ("Platform " + cnt);
            }
            else
            {
                e.CancelAction = true;
            }
        }

        /// <summary>
        /// Called when a new item has been created and is about to be entered into the list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void fontListEditor_AddingItem(object sender, ListEditor<GUIFont>.ListEditorEventArgs e)
        {
            ListEditor<GUIFont> editor = sender as ListEditor<GUIFont>;
            GUIFont font = e.Object as GUIFont;

            if (editor != null && font != null)
            {
                // Save the default font and assign it.  This will ensure we don't accidentally add blank fonts.
                SaveResource("Otter.Editor.res.LiberationSans.ttf", GUIProject.CurrentProject.ProjectDirectory + "/Fonts/LiberationSans.ttf");

                // This is a new font.  Give it a unique name
                int cnt = 1;
                int maxID = 0;
                foreach (GUIFont existingFont in editor.List)
                {
                    if (existingFont.Name == ("Font " + cnt))
                        cnt++;

                    if (existingFont.ID > maxID)
                        maxID = existingFont.ID;
                }

                font.Name = ("Font " + cnt);
                font.ID = maxID + 1;
                font.Project = GUIProject.CurrentProject;
                font.Filename = "Fonts/LiberationSans.ttf";

                font.AddASCII();
            }
            else
            {
                e.CancelAction = true;
            }
        }
        #endregion

        #region Event Handlers : Graphics
        void Graphics_OnTextureLoaded(int textureID)
        {
            this.BeginInvoke(new MethodInvoker(() =>
            {
                SceneView sceneView = mDockPanel.ActiveDocument as SceneView;
                if (sceneView != null)
                    sceneView.Refresh();
            }));           
        }

        void Graphics_OnTextureUnloaded(int textureID)
        {
            this.BeginInvoke(new MethodInvoker(() =>
            {
                SceneView sceneView = mDockPanel.ActiveDocument as SceneView;
                if (sceneView != null)
                    sceneView.Refresh();
            }));     
        }
        #endregion

        #region Overrides
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public OtterEditorMainForm(string defaultProject)
        {
            mDefaultProject = defaultProject;
            mDeserializeDockContent = new DeserializeDockContent(GetContentFromPersistString);

            Globals.PropertiesView = new PropertiesView();
            Globals.ProjectView = new ProjectView();
            Globals.TimelineView = new TimelineView();
            Globals.ControlsView = new ControlsView();
            Globals.SceneHierarchyView = new SceneHierarchyView();
            Globals.HistoryView = new HistoryView();

            InitializeComponent();
        }

        /// <summary>
        /// Sets up the plugins and adds them to the appropriate menus.
        /// </summary>
        private void SetupPlugins()
        {
            foreach (Type type in Globals.Plugins)
            {
                PluginAttribute attribute = (PluginAttribute)System.Attribute.GetCustomAttribute(type, typeof(PluginAttribute));
                if (attribute == null)
                    continue;

                ToolStripItem item = new ToolStripMenuItem(attribute.Name);
                item.Tag = type;
                item.Click += new EventHandler(PluginToolStripMenuItem_Click);

                mPluginsToolStripMenuItem.DropDown.Items.Add(item);
            }

            mPluginsToolStripMenuItem.Enabled = (mPluginsToolStripMenuItem.DropDown.Items.Count != 0);
        }

        /// <summary>
        /// Updates the view menu's checkmarks accordingly.
        /// </summary>
        private void UpdateViewMenu()
        {
            propertiesViewToolStripMenuItem.Checked = (Globals.PropertiesView.DockState != DockState.Hidden);
            projectViewToolStripMenuItem.Checked = (Globals.ProjectView.DockState != DockState.Hidden);
            timelineViewToolStripMenuItem.Checked = (Globals.TimelineView.DockState != DockState.Hidden);
            controlsViewToolStripMenuItem.Checked = (Globals.ControlsView.DockState != DockState.Hidden);
            sceneHierarchyViewToolStripMenuItem.Checked = (Globals.SceneHierarchyView.DockState != DockState.Hidden);
            historyViewToolStripMenuItem.Checked = (Globals.HistoryView.DockState != DockState.Hidden);
        }

        /// <summary>
        /// Populates the recent items menu
        /// </summary>
        private void PopulateRecentItemsMenu()
        {
            mRecentToolStripMenuItem.DropDownItems.Clear();

            if (Otter.Editor.Properties.Settings.Default.RecentProjects == null)
            {
                mRecentToolStripMenuItem.Enabled = false;
                return;
            }

            foreach (string str in Otter.Editor.Properties.Settings.Default.RecentProjects)
            {
                ToolStripMenuItem item = new ToolStripMenuItem();
                item.Text = System.IO.Path.GetFileName(str);
                item.Tag = str;
                item.Click += new EventHandler(RecentItem_Click);
                mRecentToolStripMenuItem.DropDownItems.Add(item);
            }

            mRecentToolStripMenuItem.Enabled = (Otter.Editor.Properties.Settings.Default.RecentProjects.Count != 0);
        }

        /// <summary>
        /// During layout deserialization this function will return an object
        /// corresponding to the "persistString" parameter.
        /// </summary>
        /// <param mName="persistString"></param>
        /// <returns></returns>
        private IDockContent GetContentFromPersistString(string persistString)
        {
            if (persistString == typeof(PropertiesView).ToString())
                return Globals.PropertiesView;
            else if (persistString == typeof(ProjectView).ToString())
                return Globals.ProjectView;
            else if (persistString == typeof(TimelineView).ToString())
                return Globals.TimelineView;
            else if (persistString == typeof(ControlsView).ToString())
                return Globals.ControlsView;
            else if (persistString == typeof(SceneHierarchyView).ToString())
                return Globals.SceneHierarchyView;
            else if (persistString == typeof(HistoryView).ToString())
                return Globals.HistoryView;

            return null;
        }

        /// <summary>
        /// Opens a scene for editing
        /// </summary>
        /// <param name="scene"></param>
        private void OpenEntry(GUIProjectEntry entry)
        {
            GUIProjectScene projectScene = entry as GUIProjectScene;
            SceneView sceneView = null;

            // Check to see if the document is already open, and focus
            // on it if so.
            foreach (DockContent doc in mDockPanel.Documents)
            {
                SceneView view = doc as SceneView;
                if (view != null && view.Scene == projectScene.Scene)
                {
                    doc.Show();
                    return;
                }
            }

            // We don't have a sceneview for this scene, create and 
            // add it now.
            if (sceneView == null)
            {
                LoadResources(projectScene);

                sceneView = new SceneView(projectScene.Scene);
                sceneView.TabText = projectScene.Name;

                sceneView.SelectedControlsChanged += new SceneView.ControlsEventHandler(SceneView_ControlsSelectionChanged);
                sceneView.ActiveViewChanged += new SceneView.ViewEventHandler(SceneView_ActiveViewChanged);
                sceneView.GUIControlUpdated += new SceneView.ControlEventHandler(SceneView_ControlUpdated);
                sceneView.Enter += new EventHandler(SceneView_Enter);
                sceneView.FormClosing += new FormClosingEventHandler(SceneView_FormClosing);
                sceneView.FormClosed += new FormClosedEventHandler(SceneView_FormClosed);
                
                sceneView.Show(mDockPanel, DockState.Document);
            }
        }

        private static void LoadResources(GUIProjectScene projectScene)
        {
            List<Otter.UI.Resources.Resource> resources = new List<Otter.UI.Resources.Resource>();

            resources.AddRange(projectScene.Scene.Textures.OfType<Otter.UI.Resources.Resource>());
            resources.AddRange(projectScene.Scene.Sounds.OfType<Otter.UI.Resources.Resource>());

            if (resources.Count == 0)
                return;

            LoadingForm form = new LoadingForm();

            form.ProgressBar.Minimum = 0;
            form.ProgressBar.Maximum = resources.Count;
            form.ProgressBar.Value = 0;
            form.Status.Text = "";

            form.Action = () =>
            {
                int cnt = 1;
                foreach (Otter.UI.Resources.Resource resource in resources)
                {
                    form.Status.Text = "[" + (cnt++) + "/" + resources.Count + "] : " + resource;

                    Thread thread = new Thread(new ThreadStart(() => { resource.Load(); }));
                    thread.Start();

                    while (thread.IsAlive)
                        Application.DoEvents();

                    form.ProgressBar.Value++;
                }
            };

            form.ShowDialog();
        }

        private static void UnloadResources(SceneView sceneView)
        {
            List<Otter.UI.Resources.Resource> resources = new List<Otter.UI.Resources.Resource>();

            resources.AddRange(sceneView.Scene.Textures.OfType<Otter.UI.Resources.Resource>());
            resources.AddRange(sceneView.Scene.Sounds.OfType<Otter.UI.Resources.Resource>());

            foreach (Otter.UI.Resources.Resource res in resources)
                res.Unload();
        }

        /// <summary>
        /// Removes the project from the recents list.
        /// </summary>
        /// <param name="filename"></param>
        private void RemoveProjectFromRecents(string filename)
        {
            if (Otter.Editor.Properties.Settings.Default.RecentProjects == null)
                return;

            Otter.Editor.Properties.Settings settings = Otter.Editor.Properties.Settings.Default;
            if (settings.RecentProjects.Contains(filename))
                settings.RecentProjects.Remove(filename);

            settings.Save();

            PopulateRecentItemsMenu();

        }

        /// <summary>
        /// Saves the project path to recents
        /// </summary>
        /// <param name="filename"></param>
        private void SaveProjectToRecents(string filename)
        {
            if (Otter.Editor.Properties.Settings.Default.RecentProjects == null)
                Otter.Editor.Properties.Settings.Default.RecentProjects = new System.Collections.Specialized.StringCollection();

            Otter.Editor.Properties.Settings settings = Otter.Editor.Properties.Settings.Default;
            if (settings.RecentProjects.Contains(filename))
                settings.RecentProjects.Remove(filename);

            settings.RecentProjects.Insert(0, filename);
            settings.Save();

            PopulateRecentItemsMenu();
        }

        /// <summary>
        /// Saves a resource to disk
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="filepath"></param>
        private void SaveResource(string resourceName, string filepath)
        {
            Stream stream = null;
            FileStream fs = null;
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                stream = assembly.GetManifestResourceStream(resourceName);

                byte[] data = new byte[stream.Length];
                stream.Read(data, 0, (int)stream.Length);

                if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(filepath)))
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filepath));

                fs = new FileStream(filepath, FileMode.Create, FileAccess.Write);
                fs.Write(data, 0, data.Length);
            }
            catch (Exception)
            {
            }
            finally
            {
                if (fs != null)
                    fs.Close();

                if (stream != null)
                    stream.Close();
            }
        }
        
        /// <summary>
        /// Creates a new project and saves it to disk
        /// </summary>
        private bool CreateNewProject()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "OtterUI Project files (*.ggp)|*.ggp|All files (*.*)|*.*";
            saveFileDialog.InitialDirectory = System.IO.Directory.GetCurrentDirectory();
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() != DialogResult.OK)
                return false;

            CloseWelcomeForm();

            IDockContent[] docArray = mDockPanel.Documents.ToArray();
            foreach (DockContent doc in docArray)
                doc.Close();

            GUIProject.CurrentProject = new GUIProject();
            GUIProject.CurrentProject.Name = System.IO.Path.GetFileNameWithoutExtension(saveFileDialog.FileName);
            GUIProject.CurrentProject.ProjectDirectory = System.IO.Path.GetDirectoryName(saveFileDialog.FileName);

            // Create a default platform
            Platform platform = new Platform("PC");
            platform.OutputDirectory = "Output";
            GUIProject.CurrentProject.Platforms.Add(platform);

            // Create a default resolution
            Resolution resolution = new Resolution(1024, 768);
            GUIProject.CurrentProject.Resolutions.Add(resolution);

            Globals.ProjectView.Project = GUIProject.CurrentProject;
            mProjectToolStripMenuItem.Enabled = (GUIProject.CurrentProject != null);

            // Create a default font
            SaveResource("Otter.Editor.res.LiberationSans.ttf", GUIProject.CurrentProject.ProjectDirectory + "/Fonts/LiberationSans.ttf");

            GUIFont font = new GUIFont();
            font.Name = ("Default Font");
            font.ID = 1;
            font.FontSize = 32;
            font.Project = GUIProject.CurrentProject;
            font.AddASCII();
            font.Filename = "Fonts/LiberationSans.ttf";
            GUIProject.CurrentProject.Fonts.Add(font);

            // Create a new scene
            GUIProjectScene sceneEntry = Globals.ProjectView.AddScene();

            if (sceneEntry != null)
            {
                Globals.ProjectView.OpenEntry(sceneEntry);
            }

            SaveProject(saveFileDialog.FileName);
            SaveProjectToRecents(saveFileDialog.FileName);

            return true;
        }

        /// <summary>
        /// Browses to and opens a project from file
        /// </summary>
        private bool OpenProject()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "OtterUI Project files (*.ggp)|*.ggp|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = System.IO.Directory.GetCurrentDirectory();
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return false;

            CloseWelcomeForm();
            return OpenProject(openFileDialog.FileName);
        }

        /// <summary>
        /// Opens a project
        /// </summary>
        /// <param name="filename"></param>
        private bool OpenProject(string filename)
        {
            IDockContent[] docArray = mDockPanel.Documents.ToArray();
            foreach (DockContent doc in docArray)
                doc.Close();

            GUIProject.CurrentProject = GUIProject.Load(filename);

            if (GUIProject.CurrentProject == null)
            {
                RemoveProjectFromRecents(filename);
                mProjectToolStripMenuItem.Enabled = false;
                return false;
            }

            CloseWelcomeForm();

            Globals.ProjectView.Project = GUIProject.CurrentProject;
            mProjectToolStripMenuItem.Enabled = true;

            SaveProjectToRecents(filename);

            return true;
        }

        /// <summary>
        /// Saves the project to its default location.  If it does not have a default
        /// location, brings up the save-file dialog.
        /// </summary>
        private bool SaveProject()
        {
            if (!System.IO.File.Exists(GUIProject.CurrentProject.FullPath))
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "OtterUI Project files (*.ggp)|*.ggp|All files (*.*)|*.*";
                saveFileDialog.InitialDirectory = System.IO.Directory.GetCurrentDirectory();
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    GUIProject.CurrentProject.Name = System.IO.Path.GetFileNameWithoutExtension(saveFileDialog.FileName);
                    GUIProject.CurrentProject.ProjectDirectory = System.IO.Path.GetDirectoryName(saveFileDialog.FileName);
                }
                else
                {
                    return false;
                }
            }

            return SaveProject(GUIProject.CurrentProject.FullPath);
        }

        /// <summary>
        /// Saves the project to a specific location.
        /// </summary>
        /// <param name="fileName"></param>
        private bool SaveProject(string fileName)
        {
            bool bSuccess = true;

            try
            {
                bSuccess = GUIProject.Save(GUIProject.CurrentProject, fileName);

                if (bSuccess && GUIProject.CurrentProject.FullPath != fileName)
                {
                    GUIProject.CurrentProject.Name = System.IO.Path.GetFileNameWithoutExtension(fileName);
                    GUIProject.CurrentProject.ProjectDirectory = System.IO.Path.GetDirectoryName(fileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save project.  Reason: " + ex.Message);
                bSuccess = false;
            }

            IDockContent[] docArray = mDockPanel.Documents.ToArray();
            foreach (IDockContent content in docArray)
            {
                SceneView sceneView = content as SceneView;
                if (sceneView != null)
                    sceneView.Modified = false;
            }

            return bSuccess;
        }

        /// <summary>
        /// Checks if we can save the current project.  If we are referencing any custom
        /// controls that have not been loaded, we either fail silently or prompt the user
        /// to continue
        /// </summary>
        /// <param name="promptUser"></param>
        /// <returns></returns>
        private bool CanSaveProject(bool promptUser)
        {
            bool bCanSave = true;  

            // Check to see if we can save the project.  We must prompt the user
            // if any open scenes are missing plugins
            List<string> missingCustomControls = new List<string>();
            foreach (GUIProjectScene projectScene in GUIProject.CurrentProject.Entries.OfType<GUIProjectScene>())
            {
                if (projectScene.Scene != null)
                {
                    foreach (string missingCustromControl in projectScene.Scene.MissingCustomControls)
                    {
                        if (!missingCustomControls.Contains(missingCustromControl))
                            missingCustomControls.Add(missingCustromControl);
                    }
                }
            }
            if (missingCustomControls.Count > 0)
            {
                bCanSave = false;

                if (promptUser)
                {
                    DialogResult result = MessageBox.Show(
                        "One or more scenes are missing required custom controls.  Your project may not be saved correctly.  Do you wish to continue?",
                        "Warning",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                        bCanSave = true;
                }
            }

            return bCanSave;
        }

        /// <summary>
        /// Exports the specified projectScene to disk.
        /// </summary>
        /// <param name="scene"></param>
        private void ExportScene(GUIProjectScene projectScene)
        {
            if(Otter.Editor.Properties.Settings.Default.SaveOnExport && CanSaveProject(false))
                SaveProject();

            ArrayList itemsToExport = ImporterExporter.GetItemsToExport(projectScene);    

            Exporting.AssetExporter assetExporter = new Exporting.AssetExporter(GUIProject.CurrentProject, itemsToExport);
            assetExporter.ShowDialog();
        }

        /// <summary>
        /// Selects a single control
        /// </summary>
        /// <param name="source"></param>
        /// <param name="view"></param>
        /// <param name="control"></param>
        private void SelectControl(object source, GUIView view, GUIControl control)
        {
            SelectControls(source, view, (control != null) ? new List<GUIControl>(new GUIControl[] { control }) : new List<GUIControl>());
        }

        /// <summary>
        /// Selects a list of controls, and updates all views to reflect that selection
        /// </summary>
        /// <param name="control"></param>
        private void SelectControls(object source, GUIView view, List<GUIControl> controls)
        {
            mIgnoreSelectionChanges = true;
            {
                NotifyingList<GUIControl> selectedControls = new NotifyingList<GUIControl>(controls);

                // Set the current scene's selected control, as well as the properties and
                // the scene hierarchy
                SceneView sceneView = mDockPanel.ActiveDocument as SceneView;
                if (source != sceneView && sceneView != null && sceneView.ActiveView == view)
                {
                    sceneView.SelectedControls = selectedControls;
                }
                
                if (source != Globals.TimelineView)
                    Globals.TimelineView.SelectedControls = selectedControls;

                if (source != Globals.SceneHierarchyView)
                    Globals.SceneHierarchyView.SelectedControls = selectedControls;

                if (source != Globals.PropertiesView)
                    Globals.PropertiesView.PropertyGrid.SelectedObjects = selectedControls.ToArray();
            }
            mIgnoreSelectionChanges = false;
        }

        /// <summary>
        /// Closes the welcome form if opened.
        /// </summary>
        private void CloseWelcomeForm()
        {
            if (mWelcomeForm != null)
            {
                mWelcomeForm.Close();
                mWelcomeForm = null;
            }
        }

        private void aboutOtterUIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm aboutForm = new AboutForm();
            aboutForm.ShowDialog();
        }

        private void mOnlineHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(Otter.Editor.Properties.Settings.Default.HelpURL);
            }
            catch (Exception exc1)
            {
                // System.ComponentModel.Win32Exception is a known exception that occurs when Firefox is default browser.  
                // It actually opens the browser but STILL throws this exception so we can just ignore it.  If not this exception,
                // then attempt to open the URL in IE instead.
                if (!(exc1 is Win32Exception))
                {
                    // sometimes throws exception so we have to just ignore
                    // this is a common .NET bug that no one online really has a great reason for so now we just need to try to open
                    // the URL using IE if we can.
                    try
                    {
                        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo("IExplore.exe", Otter.Editor.Properties.Settings.Default.HelpURL);
                        System.Diagnostics.Process.Start(startInfo);
                        startInfo = null;
                    }
                    catch (Exception)
                    {
                        // still nothing we can do so just show the error to the user here.
                    }
                }

            }
        }
    }
}
