using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI.Docking;

using Otter.Project;

namespace Otter.Editor.ContentViews
{
    public partial class ProjectView : DockContent
    {
        #region Events and Delegates
        public delegate void EntryEventHandler(object sender, GUIProjectEntry entry);
        public delegate void EntryActionEventHandler(object sender, ProjectEntryActionEventArgs args);

        public event EntryEventHandler OnCreateEntry = null;
        public event EntryEventHandler OnDeleteEntry = null;
        public event EntryEventHandler OnRenameEntry = null;
        public event EntryEventHandler OnOpenEntry = null;
        public event EntryActionEventHandler OnClosingEntry = null;
        public event EntryEventHandler OnCloseEntry = null;
        public event EntryEventHandler OnSelectEntry = null;
        public event EntryEventHandler OnExportEntry = null;
        #endregion

        #region Data
        private GUIProject mProject = null;
        #endregion

        #region Properties
        /// <summary>
        /// Gets / Sets the viewed project
        /// </summary>
        public GUIProject Project
        {
            get { return mProject; }
            set
            {
                if (mProject != value)
                {
                    mProject = value;

                    RefreshProjectView();
                }
            }
        }

        /// <summary>
        /// Gets / Sets the project modified flag
        /// </summary>
        public bool ProjectModified
        {
            get;
            set;
        }
        #endregion

        #region Event Handlers : Context Menu
        /// <summary>
        /// Called when the context menu strip is opening. Determine which options we need
        /// to enable and disable.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mProjectViewContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            mAddSceneToolStripMenuItem.Enabled = (GUIProject.CurrentProject != null);
            mDeleteSceneToolStripMenuItem.Enabled = (GUIProject.CurrentProject != null);
            mExportSceneToolStripMenuItem.Enabled = (GUIProject.CurrentProject != null);

            if (mScenesListView.SelectedIndices.Count == 1)
            {
                GUIProjectEntry entry = mScenesListView.SelectedItems[0].Tag as GUIProjectEntry;

                mOpenToolStripMenuItem.Enabled = (entry != null && !entry.IsOpen());
                mCloseToolStripMenuItem.Enabled = (entry != null && entry.IsOpen());
                mRenameToolStripMenuItem.Enabled = (entry != null);
            }
            else
            {
                mOpenToolStripMenuItem.Enabled = false;
                mCloseToolStripMenuItem.Enabled = false;
                mRenameToolStripMenuItem.Enabled = false;
            }
        }

        /// <summary>
        /// User chose to open a scene
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mOpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mScenesListView.SelectedIndices.Count == 1)
            {
                GUIProjectEntry entry = mScenesListView.SelectedItems[0].Tag as GUIProjectEntry;
                OpenEntry(entry);
            }
        }

        /// <summary>
        /// Closes an open entry
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mCloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mScenesListView.SelectedIndices.Count == 1)
            {
                GUIProjectEntry entry = mScenesListView.SelectedItems[0].Tag as GUIProjectEntry;
                CloseEntry(entry);
            }
        }

        /// <summary>
        /// User chose to add a new scene
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mAddSceneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddScene();
        }

        /// <summary>
        /// User chose to delete a scene
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mDeleteSceneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteSelectedScene();
        }

        /// <summary>
        /// User chose to rename a scene
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mRenameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RenameSelectedScene();
        }

        /// <summary>
        /// User chose to export a scene
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mExportSceneToolStripMenuItem_Click(object sender, EventArgs e)
        {            
            foreach(ListViewItem item in mScenesListView.SelectedItems)
            {
                GUIProjectEntry entry = item.Tag as GUIProjectEntry;
                if (entry != null)
                {
                    NotifyExportEntry(entry);
                }
            }
        }
        #endregion

        #region Event Handlers : List ParentView
        /// <summary>
        /// The selected item has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mScenesListView_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// User has pressed a key.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mScenesListView_KeyDown(object sender, KeyEventArgs e)
        {
            // F2 : Renames a scene
            if (e.KeyCode == Keys.F2)
            {
                RenameSelectedScene();
            }
        }

        /// <summary>
        /// Occurs after the user has edited the label.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mScenesListView_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (mProject == null)
                return;

            // Bail if the edit was cancelled (label is null) or new name is empty
            if (e.Label == null || e.Label == "")
            {
                e.CancelEdit = true;
                mScenesListView.LabelEdit = false;
                return;
            }

            // Bail if the projectscene was not attached to the item.
            GUIProjectScene projectScene = mScenesListView.Items[e.Item].Tag as GUIProjectScene;
            if (projectScene == null)
            {
                e.CancelEdit = true;
                mScenesListView.LabelEdit = false;
                return;
            }

            // Make sure no other scenes have the same name.
            foreach (GUIProjectScene prjScene in mProject.Entries)
            {
                if (prjScene.Name == e.Label)
                {
                    e.CancelEdit = true;
                    mScenesListView.LabelEdit = false;
                    return;
                }
            }

            // Finally try to rename it.
            if (!projectScene.Rename(e.Label))
            {
                e.CancelEdit = true;
            }
            else
            {
                this.ProjectModified = true;
                NotifyRenameEntry(projectScene);
            }

            mScenesListView.LabelEdit = false;
        }

        /// <summary>
        /// Called when the user double-clicks on the control.  See if we
        /// double-clicked on an item.  If so, notify.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mScenesListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo hit = mScenesListView.HitTest(e.Location);
            if (hit.Item != null)
            {
                GUIProjectEntry entry = hit.Item.Tag as GUIProjectEntry;
                OpenEntry(entry);
            };
        }
        #endregion

        #region Event Notifications
        /// <summary>
        /// Notifies when an entry has been created
        /// </summary>
        /// <param name="entry"></param>
        private void NotifyCreateEntry(GUIProjectEntry entry)
        {
            if (OnCreateEntry != null)
                OnCreateEntry(this, entry);
        }

        /// <summary>
        /// Notifies when an entry has been created
        /// </summary>
        /// <param name="entry"></param>
        private void NotifyDeleteEntry(GUIProjectEntry entry)
        {
            if (OnDeleteEntry != null)
                OnDeleteEntry(this, entry);
        }

        /// <summary>
        /// Notifies when an entry has been renamed
        /// </summary>
        /// <param name="entry"></param>
        private void NotifyRenameEntry(GUIProjectEntry entry)
        {
            if (OnRenameEntry != null)
                OnRenameEntry(this, entry);
        }

        /// <summary>
        /// Notifies when an entry has been opened
        /// </summary>
        /// <param name="entry"></param>
        private void NotifySelectEntry(GUIProjectEntry entry)
        {
            if (OnSelectEntry != null)
                OnSelectEntry(this, entry);
        }

        /// <summary>
        /// Notifies when an entry has been opened
        /// </summary>
        /// <param name="entry"></param>
        private void NotifyOpenEntry(GUIProjectEntry entry)
        {
            if (OnOpenEntry != null)
                OnOpenEntry(this, entry);
        }

        /// <summary>
        /// Notifies that we are about to close an entry
        /// </summary>
        /// <param name="entry"></param>
        private bool NotifyClosingEntry(GUIProjectEntry entry)
        {
            if (OnClosingEntry != null)
            {
                ProjectEntryActionEventArgs args = new ProjectEntryActionEventArgs(entry);
                OnClosingEntry(this, args);

                return args.CancelAction;
            }

            return false;
        }

        /// <summary>
        /// Notifies when an entry has been opened
        /// </summary>
        /// <param name="entry"></param>
        private void NotifyCloseEntry(GUIProjectEntry entry)
        {
            if (OnCloseEntry != null)
                OnCloseEntry(this, entry);
        }

        /// <summary>
        /// Notifies when the user has chosen to export an entry
        /// </summary>
        /// <param name="entry"></param>
        private void NotifyExportEntry(GUIProjectEntry entry)
        {
            if (OnExportEntry != null)
                OnExportEntry(this, entry);
        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public ProjectView()
        {
            InitializeComponent();

            this.HideOnClose = true;
        }

        /// <summary>
        /// Refreshes the project sceneView.  Reconstructs all
        /// lists and other controls.
        /// </summary>
        public void RefreshProjectView()
        {
            mScenesListView.Items.Clear();

            if (mProject == null)
                return;

            foreach (GUIProjectScene projectScene in mProject.Entries)
            {
                ListViewItem item = new ListViewItem(projectScene.Name, 0);
                item.Tag = projectScene;
                mScenesListView.Items.Add(item);
            }
        }

        /// <summary>
        /// Adds a new scene to the project
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public GUIProjectScene AddScene()
        {
            if(mProject == null)
                return null;

            string name = "NewScene";
            int cnt = 0;

            GUIProjectScene newSceneEntry = new GUIProjectScene(name + cnt);

            bool bUnique = false;
            while(!bUnique)
            {
                bUnique = true;

                cnt++;
                newSceneEntry.Name = name + cnt;

                // Check if it already exists on disk.
                if(System.IO.File.Exists(newSceneEntry.FullPath))
                {
                    bUnique = false;
                }

                // If it does not exist on disk, make sure there isn't
                // another entry by the same name
                if(bUnique)
                {
                    foreach (GUIProjectScene projectScene in mProject.Entries)
                    {
                        if (projectScene.Name == newSceneEntry.Name)
                        {
                            bUnique = false;
                            break;
                        }
                    }
                }
            };

            // Cycle through the entries and determine the max id.
            uint maxID = 0;
            foreach (GUIProjectEntry entry in mProject.Entries)
            {
                if (entry.ID > maxID)
                    maxID = entry.ID;
            }

            newSceneEntry.ID = maxID + 1;
                        
            // Save it the scene.
            newSceneEntry.Save();
            mProject.Entries.Add(newSceneEntry);
            this.ProjectModified = true;

            RefreshProjectView();

            NotifyCreateEntry(newSceneEntry);

            return newSceneEntry;
        }

        /// <summary>
        /// Opens an existing entry
        /// </summary>
        /// <param name="entry"></param>
        public void OpenEntry(GUIProjectEntry entry)
        {
            if (entry != null)
            {
                if (entry.Open())
                {
                    NotifyOpenEntry(entry);
                }
                else
                {
                    MessageBox.Show("Failed to load " + entry.Filename + ".", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Closes the entry
        /// </summary>
        /// <param name="entry"></param>
        private bool CloseEntry(GUIProjectEntry entry)
        {
            if (entry == null || NotifyClosingEntry(entry))
                return false;

            if (entry.Close())
                NotifyCloseEntry(entry);

            return true;
        }

        /// <summary>
        /// Deletes the currently selected scenes from the project
        /// </summary>
        private void DeleteSelectedScene()
        {
            if (mScenesListView.SelectedIndices.Count > 0)
            {
                DialogResult res = MessageBox.Show( Otter.Editor.Properties.Resources.DELETE_SCENE,
                                                    Otter.Editor.Properties.Resources.WARNING, 
                                                    MessageBoxButtons.YesNo, 
                                                    MessageBoxIcon.Exclamation);
                if (res == DialogResult.No)
                    return;

                foreach (ListViewItem item in mScenesListView.SelectedItems)
                {
                    GUIProjectScene sceneEntry = item.Tag as GUIProjectScene;

                    if (sceneEntry != null)
                    {
                        if (sceneEntry.IsOpen() && !CloseEntry(sceneEntry))
                            continue;

                        sceneEntry.Delete();
                        mProject.Entries.Remove(sceneEntry);
                        NotifyDeleteEntry(sceneEntry);

                        this.ProjectModified = true;
                    }
                }

                mScenesListView.SelectedItems.Clear();
                RefreshProjectView();
            }
        }

        /// <summary>
        /// Renames the selected scene
        /// </summary>
        private void RenameSelectedScene()
        {
            if (mScenesListView.SelectedIndices.Count == 1)
            {
                mScenesListView.LabelEdit = true;
                mScenesListView.SelectedItems[0].BeginEdit();
            }
        }
    }

    /// <summary>
    /// Arguments when a GUI Project Entry action is about to occur.
    /// Action can be prevented by setting "Cancel" to true
    /// </summary>
    public class ProjectEntryActionEventArgs
    {
        #region Data
        private GUIProjectEntry mEntry = null;
        private bool mCancel = false;
        #endregion

        /// <summary>
        /// Gets the entry that the action applies to
        /// </summary>
        public GUIProjectEntry Entry
        {
            get { return mEntry; }
        }

        /// <summary>
        /// Gets / Sets whether or not to cancel this action
        /// </summary>
        public bool CancelAction
        {
            get { return mCancel; }
            set { mCancel = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="entry"></param>
        public ProjectEntryActionEventArgs(GUIProjectEntry entry)
        {
            mEntry = entry;
        }
    }
}
