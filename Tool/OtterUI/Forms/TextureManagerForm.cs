using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

using Otter.Project;
using Otter.UI.Resources;

namespace Otter.Forms
{
    public partial class TextureManagerForm : Form
    {
        #region Structures
        class Reference
        {
            public UI.GUIView View = null;
            public List<UI.GUIControl> Controls = new List<UI.GUIControl>();
        }
        #endregion

        #region Data
        private UI.GUIScene mScene = null;
        private bool mSelectMode = false;
        private bool mPopulatingInfo = false;
        #endregion

        #region Properties
        /// <summary>
        /// Gets / Sets the selected texture
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public UI.Resources.TextureInfo SelectedTexture
        {
            get { return mTextureList.SelectedTexture; }
            set { mTextureList.SelectedTexture = value; }
        }
        #endregion

        #region Event Handlers : Texture Manager Form
        /// <summary>
        /// Calls when the texture manager form has been loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextureManagerForm_Load(object sender, EventArgs e)
        {
            if (this.DesignMode)
                return;

            mTextureList.AllowNullTexture = mSelectMode;

            mOKButton.Visible = mSelectMode;
            mCloseButton.Text = mSelectMode ? "Cancel" : "Close";
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
            this.mTextureList.Dispose();

            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        #endregion

        #region Event Handlers : Texture List
        /// <summary>
        /// Called when the selected texture has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="info"></param>
        private void mTextureList_SelectedTextureChanged(object sender, Otter.UI.Resources.TextureInfo info)
        {
            mDetailsGroupBox.Enabled = (info != null);

            mReferencesListView.Groups.Clear();
            mReferencesListView.Items.Clear();
            mFilenameTextBox.Text = "";
            mRefreshButton.Enabled = (info != null);

            if (info != null)
            {
                if (info.TextureID != -1)
                {
                    mWidthTextBox.Text = info.Width.ToString();
                    mHeightTextBox.Text = info.Height.ToString();
                }
                else
                {
                    mWidthTextBox.Text = "(unknown)";
                    mHeightTextBox.Text = "(unknown)";
                }
                
                UpdateInfo(info);
            }
        }

        /// <summary>
        /// Updates the texture information
        /// </summary>
        /// <param name="info"></param>
        private void UpdateInfo(Otter.UI.Resources.TextureInfo info)
        {
            if (info == null)
            {
                mDetailsGroupBox.Enabled = false;
                mAtlasCheckbox.Checked = false;
                return;
            }

            System.IO.FileInfo fileInfo = new System.IO.FileInfo(GUIProject.CurrentProject.ProjectDirectory + "/" + info.Filename);

            mSizeTextBox.Text = fileInfo.Exists ? String.Format("{0:N0}", fileInfo.Length) : "(file not found)";
            mFilenameTextBox.Text = info.Filename;

            mPopulatingInfo = true;
            {
                mAtlasCheckbox.Checked = !info.NoAtlas;
                mOverrideNumericUpDown.Value = (info.AtlasPadding >= 0) ? info.AtlasPadding : 0;
                mOverrideCheckbox.Checked = info.AtlasPadding >= 0;
                mPaddingTypeComboBox.SelectedItem = info.AtlasBorderType;

                mOverrideNumericUpDown.Enabled = mAtlasCheckbox.Checked && mOverrideCheckbox.Checked;
                mOverrideCheckbox.Enabled = mAtlasCheckbox.Checked;
                mPaddingTypeComboBox.Enabled = mAtlasCheckbox.Checked;
                mPaddingTypeLabel.Enabled = mAtlasCheckbox.Checked;

                Reference[] references = GetReferences(info);
                foreach (Reference reference in references)
                {
                    ListViewGroup group = new ListViewGroup(reference.View.Name);
                    group.Tag = reference.View;

                    mReferencesListView.Groups.Add(group);

                    foreach (UI.GUIControl control in reference.Controls)
                    {
                        ListViewItem item = new ListViewItem(control.Name);
                        item.Tag = control;
                        item.Group = group;

                        mReferencesListView.Items.Add(item);
                    }
                }

                mRemoveButton.Enabled = (references.Length == 0);
            }
            mPopulatingInfo = false;
        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="scene"></param>
        public TextureManagerForm(UI.GUIScene scene, bool selectMode)
        {
            mSelectMode = selectMode;

            InitializeComponent();

            mReferencesListView.Groups.Clear();
            mReferencesListView.Items.Clear();

            mScene = scene;
            mTextureList.AllowNullTexture = mSelectMode;
            if (mScene != null)
                mTextureList.Textures = mScene.Textures.ToArray();

            mRefreshButton.Enabled = false;

            mPaddingTypeComboBox.Items.Add(TextureInfo.AtlasBorder.Default);
            mPaddingTypeComboBox.Items.Add(TextureInfo.AtlasBorder.Clear);
            mPaddingTypeComboBox.Items.Add(TextureInfo.AtlasBorder.RepeatPixel);
        }

        /// <summary>
        /// Browses for a new texture
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrowseButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.pcx, *.jpg, *.png, *.tga)|*.pcx;*.jpg;*.png;*.tga|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = GUIProject.CurrentProject.ProjectDirectory;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filename = Utils.GetRelativePath(GUIProject.CurrentProject.ProjectDirectory, openFileDialog.FileName);

                foreach (UI.Resources.TextureInfo info in mScene.Textures)
                {
                    if (info.Filename == filename)
                    {
                        MessageBox.Show("This texture is already used.");
                        return;
                    }
                }

                // Close the existing texture
                mTextureList.SelectedTexture.Unload();

                // Change the texture fileName
                mTextureList.SelectedTexture.Filename = filename;

                // Finally load the texture.
                mTextureList.SelectedTexture.Load();

                UpdateInfo(mTextureList.SelectedTexture);

                mTextureList.Invalidate(true);
            }
        }

        /// <summary>
        /// Retrieves an array of controls that reference a particular texture.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private Reference[] GetReferences(UI.Resources.TextureInfo info)
        {
            List<Reference> list = new List<Reference>();

            if (info != null)
            {
                foreach (UI.GUIView view in mScene.Views)
                {
                    Reference reference = null;
                    foreach (UI.GUIControl control in view.Controls)
                    {
                        if (control.TextureIDs.Contains(info.ID))
                        {
                            if(reference == null)
                            {
                                reference = new Reference();
                                reference.View = view;
                            }

                            reference.Controls.Add(control);
                        }                        
                    }

                    if (reference != null)
                        list.Add(reference);
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// Adds a new texture to the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mAddButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.pcx, *.jpg, *.png, *.tga)|*.pcx;*.jpg;*.png;*.tga|All files (*.*)|*.*";
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
                foreach (UI.Resources.TextureInfo info in mScene.Textures)
                {
                    if (info.Filename == filename)
                    {
                        numPresent++;
                        continue;
                    }
                }
                
                mScene.CreateTexture(filename);
            }

            mTextureList.SelectedTexture = null;
            mTextureList.Textures = mScene.Textures.ToArray();

            if (numPresent > 0)
            {
                MessageBox.Show(numPresent.ToString() + " out of " + totalSelected + " already exists in the texture list, and were not added.", 
                                "Information", 
                                MessageBoxButtons.OK, 
                                MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Removes a texture from the scene's texture list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mRemoveButton_Click(object sender, EventArgs e)
        {
            UI.Resources.TextureInfo texToRemove = mTextureList.SelectedTexture;

            if (texToRemove == null)
                return;

            Reference[] references = GetReferences(texToRemove);
            if (references.Length > 0)
                return;

            mTextureList.SelectedTexture = null;

            mScene.Textures.Remove(texToRemove);
            mTextureList.Textures = mScene.Textures.ToArray();
        }

        /// <summary>
        /// Refreshes the selected texture
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mRefreshButton_Click(object sender, EventArgs e)
        {
            UI.Resources.TextureInfo texToRefresh = mTextureList.SelectedTexture;

            if (texToRefresh == null)
                return;
            
            mTextureList.UpdateTexture(texToRefresh);
        }
        
        /// <summary>
        /// Called when the checkbox has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mAtlasCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (mPopulatingInfo)
                return;

            if (mTextureList.SelectedTexture == null)
                return;

            mTextureList.SelectedTexture.NoAtlas = !mAtlasCheckbox.Checked;

            UpdateInfo(mTextureList.SelectedTexture);
        }

        private void mOverrideCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (mPopulatingInfo)
                return;

            if (mOverrideCheckbox.Checked)
            {
                mOverrideNumericUpDown.Enabled = true;
                mTextureList.SelectedTexture.AtlasPadding = (int)mOverrideNumericUpDown.Value;
            }
            else
            {
                mOverrideNumericUpDown.Enabled = false;
                mTextureList.SelectedTexture.AtlasPadding = -1;
            }
        }

        private void mOverrideNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (mPopulatingInfo)
                return;

            if (mOverrideCheckbox.Checked)
                mTextureList.SelectedTexture.AtlasPadding = (int)mOverrideNumericUpDown.Value;
        }

        private void mPaddingTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mPopulatingInfo)
                return;

            mTextureList.SelectedTexture.AtlasBorderType = (TextureInfo.AtlasBorder)mPaddingTypeComboBox.SelectedItem;
        }
    }
}
