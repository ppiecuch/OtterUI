using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

using Otter.Project;

namespace Otter.CustomControls
{
    public partial class TextureList : UserControl
    {
        #region Events and Delegates
        public delegate void TextureInfoHandler(object sender, UI.Resources.TextureInfo info);

        public event TextureInfoHandler SelectedTextureChanged = null;
        #endregion

        #region Data
        private UI.Resources.TextureInfo[] mTextures = null;
        private UI.Resources.TextureInfo mSelectedTexture = null;
        private bool mAllowNullTexture = false;

        private const int MARGIN = 10;
        private const int INNER_MARGIN = 5;
        private const int LABEL_HEIGHT = 20;
        #endregion

        #region Properties        
        /// <summary>
        /// Gets / Sets the displayed list of textures
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public UI.Resources.TextureInfo[] Textures
        {
            get { return mTextures; }
            set 
            { 
                mTextures = value;
                SelectedTexture = null;

                RebuildList();
            }
        }

        /// <summary>
        /// Gets / Sets the selected texture
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public UI.Resources.TextureInfo SelectedTexture
        {
            get { return mSelectedTexture; }
            set
            {
                UI.Resources.TextureInfo info = value;

                if (!mTextures.Contains(info))
                    info = null;

                if (mSelectedTexture != info)
                {
                    mSelectedTexture = info;
                    NotifySelectedTextureChanged(mSelectedTexture);

                    ScrollToTexture(SelectedTexture);
                    this.Invalidate(true);
                }
            }
        }

        /// <summary>
        /// Gets/Sets whether or not the texture list will display the null (-1)
        /// texture
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [DefaultValue(false)]
        public bool AllowNullTexture
        {
            get
            {
                return mAllowNullTexture;
            }
            set
            {
                mAllowNullTexture = value;
            }
        }
        #endregion

        #region Event Notifiers

        /// <summary>
        /// Notifies that the selected texture has changed
        /// </summary>
        /// <param name="info"></param>
        private void NotifySelectedTextureChanged(UI.Resources.TextureInfo info)
        {
            if (SelectedTextureChanged != null)
                SelectedTextureChanged(this, info);
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scene"></param>
        public TextureList()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Called when the texture list is loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextureList_Load(object sender, EventArgs e)
        {
            if (mSelectedTexture != null)
            {
                ListViewItem item = GetTextureListViewItem(mSelectedTexture);
                item.Selected = true;

                ScrollToTexture(SelectedTexture);
            }
        }

        /// <summary>
        /// Retrieves the ListViewItem associated with a texture info
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private ListViewItem GetTextureListViewItem(UI.Resources.TextureInfo info)
        {
            foreach (ListViewItem item in mTexturesListView.Items)
            {
                if (item.Tag == info)
                {
                    return item;
                }
            }

            return null;
        }

        /// <summary>
        /// Rebuilds the texture list 
        /// </summary>
        private void RebuildList()
        {
            mTexturesListView.Items.Clear();

            // Add (null) to the list to signal the empty texture
            if (AllowNullTexture)
            {
                mTexturesListView.Items.Add("(none)", 0);
            }

            if (Textures == null || Textures.Length == 0)
                return;

            while (mImageList.Images.Count > 1)
                mImageList.Images.RemoveAt(mImageList.Images.Count - 1);

            foreach (UI.Resources.TextureInfo tex in Textures)
            {
                ListViewItem item = new ListViewItem();
                item.Text = tex.Filename;
                item.Tag = tex;

                if (tex.Thumbnail != null)
                {
                    mImageList.Images.Add(tex.Thumbnail);
                    item.ImageIndex = mImageList.Images.Count - 1;
                }
                else
                {
                    item.ImageIndex = 0;
                }

                mTexturesListView.Items.Add(item);
            }

            ScrollToTexture(SelectedTexture);
        }

        /// <summary>
        /// Updates the texture in the texture list
        /// </summary>
        /// <param name="info"></param>
        public void UpdateTexture(UI.Resources.TextureInfo info)
        {
            ListViewItem item = GetTextureListViewItem(info);
            if (item == null)
                return;

            info.Unload();
            info.Load();

            Bitmap bmp = new Bitmap(100, 100);
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.Red);
            g.Dispose();

            mTexturesListView.LargeImageList.Images.Add(info.Thumbnail);
            item.ImageIndex = mTexturesListView.LargeImageList.Images.Count - 1;
            mTexturesListView.Invalidate(true);
        }

        /// <summary>
        /// Scrolls to a particular texture
        /// </summary>
        /// <param name="info"></param>
        private void ScrollToTexture(UI.Resources.TextureInfo info)
        {
            if (info == null)
                return;

            ListViewItem item = GetTextureListViewItem(info);
            if (item == null)
                return;

            item.EnsureVisible();
        }

        /// <summary>
        /// Called when the user clicked on a texture panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TexturePanel_Click(object sender, EventArgs e)
        {
            Control control = sender as Control;
            if (control == null)
                return;

            SelectedTexture = control.Tag as UI.Resources.TextureInfo;
        }

        private void mTexturesListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mTexturesListView.SelectedItems.Count < 1)
            {
                SelectedTexture = null;
                return;
            }

            SelectedTexture = mTexturesListView.SelectedItems[0].Tag as UI.Resources.TextureInfo;
        }
    }
}
