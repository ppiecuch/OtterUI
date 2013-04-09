using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Linq;

using Otter.Project;
using Otter.UI;
using Otter.UI.Resources;
using Otter.UI.Animation;
using Otter.UI.Actions;
namespace Otter.Forms
{
    public partial class SoundManagerForm : Form
    {
        #region Structures
        class Reference
        {
            public GUIView View = null;
            public List<GUIAnimation> Animations = new List<GUIAnimation>();
        }
        #endregion

        #region Data
        private UI.GUIScene mScene = null;
        private bool mSelectMode = false;
        #endregion

        #region Properties
        /// <summary>
        /// Gets / Sets the selected sound
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public SoundInfo SelectedSound
        {
            get
            {
                if (mSoundList.SelectedItems.Count > 0)
                {
                    ListViewItem item = mSoundList.SelectedItems[0];
                    return item.Tag as SoundInfo;
                }

                return null;
            }
        }
        
        /// <summary>
        /// Gets / Sets whether or not the sound manager form is in select
        /// mode
        /// </summary>
        public bool SelectMode
        {
            get { return mSelectMode; }
            set { mSelectMode = value; }
        }
        #endregion

        #region Event Handlers : Sound Manager Form
        /// <summary>
        /// Calls when the sound manager form has been loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SoundManagerForm_Load(object sender, EventArgs e)
        {
            if (this.DesignMode)
                return;

            mOKButton.Visible = this.SelectMode;
            mCloseButton.Text = this.SelectMode ? "Cancel" : "Close";
        }

        /// <summary>
        /// Closes the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mOKButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// Closes this form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mCloseButton_Click(object sender, EventArgs e)
        {
            this.mSoundList.Dispose();

            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        #endregion

        #region Event Handlers : Sound List
        /// <summary>
        /// Called when the selected sound has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="info"></param>
        /// 

        private void mSoundList_SelectedIndexChanged(object sender, EventArgs e)
        {
            SoundInfo info = SelectedSound;

            if (info != null)
            {
                UpdateInfo(info);
            }
        }

        /// <summary>
        /// Attempts to edit the currently selected item if the user hit F2 on the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mSoundList_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.F2)
                EditSelectedItem();
        }

        /// <summary>
        /// Called after the label has been edited
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mSoundList_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (e.Item < 0 || e.Item >= mSoundList.Items.Count)
                return;

            ListViewItem item = mSoundList.Items[e.Item];
            SoundInfo info = item.Tag as SoundInfo;
            if (info != null)
            {
                if (e.Label != null && e.Label != "" && e.Label != info.Name)
                {
                    foreach (SoundInfo soundInfo in mScene.Sounds)
                    {
                        if (soundInfo != info && soundInfo.Name == e.Label)
                        {
                            e.CancelEdit = true;
                            return;
                        }
                    }

                    info.Name = e.Label;
                    item.Name = e.Label;

                    mSoundList.Refresh();
                    mSoundList.SelectedIndices.Add(e.Item);
                }
                else
                {
                    e.CancelEdit = true;
                }
            }

        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public SoundManagerForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="scene"></param>
        public SoundManagerForm(UI.GUIScene scene)
        {
            InitializeComponent();

            mScene = scene;

            UpdateListView(scene.Sounds);
        }

        /// <summary>
        /// Updates the list view, and sets the selected sound
        /// </summary>
        /// <param name="sounds"></param>
        /// <param name="selectedSound"></param>
        private void UpdateListView(List<SoundInfo> sounds)
        {
            mSoundList.Clear();
            foreach (SoundInfo info in sounds)
            {
                ListViewItem item = new ListViewItem(info.Name);
                item.Tag = info;
                mSoundList.Items.Add(item);
            }
        }

        /// <summary>
        /// Retrieves an array of controls that reference a particular texture.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private Reference[] GetReferences(SoundInfo info)
        {
            List<Reference> list = new List<Reference>();

            if (info != null)
            {
                foreach (GUIView view in mScene.Views)
                {
                    Reference reference = null;
                    foreach (GUIAnimation animation in view.Animations)
                    {
                        bool bFound = false;
                        foreach (MainChannelFrame mainChannelFrame in animation.MainChannelFrames)
                        {
                            foreach (SoundAction soundAction in mainChannelFrame.Actions.OfType<SoundAction>())
                            {
                                if (soundAction.Sound == info.ID)
                                {
                                    bFound = true;
                                    break;
                                }
                            }

                            if (bFound)
                                break;
                        }

                        if (bFound)
                        {
                            if (reference == null)
                            {
                                reference = new Reference();
                                reference.View = view;
                            }

                            reference.Animations.Add(animation);
                        }
                    }

                    if (reference != null)
                        list.Add(reference);
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// Edits the currently selected item in the list box
        /// </summary>
        private void EditSelectedItem()
        {
            if (mSoundList.SelectedItems.Count == 1)
            {
                mSoundList.SelectedItems[0].BeginEdit();
            }
        }

        /// <summary>
        /// Updates the texture information
        /// </summary>
        /// <param name="info"></param>
        private void UpdateInfo(Otter.UI.Resources.SoundInfo info)
        {
            if (info == null)
            {
                mDetailsGroupBox.Enabled = false;

                mReferencesListView.Groups.Clear();
                mReferencesListView.Items.Clear();

                mSizeTextBox.Text = "";
                mFilenameTextBox.Text = "";

                return;
            }

            mDetailsGroupBox.Enabled = true;

            System.IO.FileInfo fileInfo = new System.IO.FileInfo(GUIProject.CurrentProject.ProjectDirectory + "/" + info.Filename);

            mSizeTextBox.Text = fileInfo.Exists ? fileInfo.Length.ToString() : "(file not found)";
            mFilenameTextBox.Text = info.Filename;

            mReferencesListView.Groups.Clear();
            mReferencesListView.Items.Clear();

            Reference[] references = GetReferences(info);
            foreach (Reference reference in references)
            {
                ListViewGroup group = new ListViewGroup(reference.View.Name);
                group.Tag = reference.View;

                mReferencesListView.Groups.Add(group);

                foreach (GUIAnimation animation in reference.Animations)
                {
                    ListViewItem item = new ListViewItem(animation.Name);
                    item.Tag = animation;
                    item.Group = group;

                    mReferencesListView.Items.Add(item);
                }
            }

            mPlayButton.Enabled = info.IsPlayable();
            mRemoveButton.Enabled = (mReferencesListView.Groups.Count == 0);
        }

        /// <summary>
        /// Browses for a new sound
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrowseButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Sound files (*.wav)|*.wav|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = GUIProject.CurrentProject.ProjectDirectory;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filename = Utils.GetRelativePath(GUIProject.CurrentProject.ProjectDirectory, openFileDialog.FileName);

                foreach (UI.Resources.SoundInfo info in mScene.Sounds)
                {
                    if (info.Filename == filename)
                    {
                        MessageBox.Show("This sound is already used.");
                        return;
                    }
                }

                SelectedSound.Filename = filename;
                SelectedSound.Load();

                UpdateInfo(SelectedSound);
            }
        }

        /// <summary>
        /// Adds a new sound to the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mAddButton_Click(object sender, EventArgs e)
        {
            string name = "Sound ";
            int cnt = 0;

            bool bUnique = true;
            do
            {
                bUnique = true;
                cnt++;
                foreach (SoundInfo info in mScene.Sounds)
                {
                    if (info.Name == (name + cnt))
                    {
                        bUnique = false;
                        break;
                    }
                }
            }
            while (!bUnique);

            int id = mScene.CreateSound(name + cnt);
            if (mScene.GetSoundInfo(id) != null)
            {
                UpdateListView(mScene.Sounds);
            }

            UpdateInfo(SelectedSound);
        }
        
        private void tmp()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Sound files (*.wav)|*.wav|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = GUIProject.CurrentProject.ProjectDirectory;
            openFileDialog.Multiselect = true;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;

            int totalSelected = openFileDialog.FileNames.Length;
            int numPresent = 0;
            foreach (string fn in openFileDialog.FileNames)
            {
                string filename = Utils.GetRelativePath(GUIProject.CurrentProject.ProjectDirectory, fn);
                foreach (UI.Resources.SoundInfo info in mScene.Sounds)
                {
                    if (info.Filename == filename)
                    {
                        numPresent++;
                        continue;
                    }
                }

                int id = mScene.CreateSound(filename);
                if (mScene.GetSoundInfo(id) != null)
                {
                    UpdateListView(mScene.Sounds);
                }
            }

            if (numPresent > 0)
            {
                MessageBox.Show(numPresent.ToString() + " out of " + totalSelected + " already exists in the sound list, and were not added.", 
                                "Information", 
                                MessageBoxButtons.OK, 
                                MessageBoxIcon.Information);
            }
        }

        /// <summary> 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mRemoveButton_Click(object sender, EventArgs e)
        {
            UI.Resources.SoundInfo soundToRemove = this.SelectedSound;

            if (soundToRemove == null)
                return;

            if (GetReferences(soundToRemove).Length > 0)
                return;

            mScene.Sounds.Remove(soundToRemove);
            UpdateListView(mScene.Sounds);
        }
        
        /// <summary>
        /// Plays the sound.  Changes the button from "Play" to "Stop" while the sound is being played
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mPlayButton_Click(object sender, EventArgs e)
        {
            SoundInfo info = this.SelectedSound;
            if (info == null || !info.IsPlayable())
                return;

            if (!info.IsPlaying())
            {
                info.Play(this);
                mPlayButton.Text = "Stop";

                info.OnStopped += new SoundInfo.OnSoundEventHandler(SoundInfo_OnStopped);
            }
            else
            {
                info.Stop();
            }
        }

        /// <summary>
        /// Called when the sound has stopped
        /// </summary>
        /// <param name="info"></param>
        void SoundInfo_OnStopped(SoundInfo info)
        {
            if (SelectedSound != info)
                return;

            mPlayButton.Text = "Play";
        }
    }
}
