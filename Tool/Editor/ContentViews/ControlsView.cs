using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI.Docking;
using Otter.UI;

namespace Otter.Editor.ContentViews
{
    public partial class ControlsView : DockContent
    {
        public delegate void ControlsViewDelegate(object sender);
        public event ControlsViewDelegate OnControlSelectionChanged;

        /// <summary>
        /// Returns the selected control type.  Returns null if no type was selected
        /// </summary>
        public Type SelectedType
        {
            get
            {
                if (mControlsListView.SelectedIndices.Count == 1)
                    return mControlsListView.SelectedItems[0].Tag as Type;

                return null;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ControlsView()
        {
            InitializeComponent();
            List<ListViewItem> items = new List<ListViewItem>();
            ListViewItem item = null;
            
            item = new ListViewItem("Sprite", 1);
            item.Group = mControlsListView.Groups[0];
            item.Tag = typeof(GUISprite);
            items.Add(item);

            item = new ListViewItem("Label", 2);
            item.Group = mControlsListView.Groups[0];
            item.Tag = typeof(GUILabel);
            items.Add(item);

            item = new ListViewItem("Button", 3);
            item.Group = mControlsListView.Groups[0];
            item.Tag = typeof(GUIButton);
            items.Add(item);

            item = new ListViewItem("Table", 4);
            item.Group = mControlsListView.Groups[0];
            item.Tag = typeof(GUITable);
            items.Add(item);

            item = new ListViewItem("Toggle", 5);
            item.Group = mControlsListView.Groups[0];
            item.Tag = typeof(GUIToggle);
            items.Add(item);

            item = new ListViewItem("Slider", 6);
            item.Group = mControlsListView.Groups[0];
            item.Tag = typeof(GUISlider);
            items.Add(item);

            item = new ListViewItem("Mask", 7);
            item.Group = mControlsListView.Groups[0];
            item.Tag = typeof(GUIMask);
            items.Add(item);

            item = new ListViewItem("Group", 0);
            item.Group = mControlsListView.Groups[0];
            item.Tag = typeof(GUIGroup);
            items.Add(item);

            foreach (Type type in Globals.CustomControlTypes)
            {
                // Grab the attributes.  Search for "ControlAttribute"
                System.Attribute attribute = System.Attribute.GetCustomAttribute(type, typeof(Otter.Plugins.ControlAttribute));
                if (attribute != null)
                {
                    Otter.Plugins.ControlAttribute controlAttribute = (Otter.Plugins.ControlAttribute)attribute;
                    Otter.Plugins.ControlDescriptor controlDescriptor = controlAttribute.GetDescriptor();

                    int imageIndex = 0;
                    if (controlDescriptor.Image != null)
                    {
                        mImageList.Images.Add(controlDescriptor.Image);
                        imageIndex = mImageList.Images.Count - 1;
                    }

                    item = new ListViewItem(controlDescriptor.Name, imageIndex);
                    item.Group = mControlsListView.Groups[1];
                    item.Tag = type;
                    items.Add(item); 
                }
            }

            mControlsListView.Items.AddRange(items.ToArray());

            this.Leave += new EventHandler(ControlsView_Leave);
            this.HideOnClose = true;
        }

        /// <summary>
        /// Called when the input focus leaves this control.  Clear the selected indices.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ControlsView_Leave(object sender, EventArgs e)
        {
            mControlsListView.SelectedIndices.Clear();
        }

        /// <summary>
        /// Called when the selected index has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mControlsListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (OnControlSelectionChanged != null)
                OnControlSelectionChanged(this);
        }

        private void mControlsListView_Resize(object sender, EventArgs e)
        {
            // int size = Math.Max(mControlsListView.Width - 30, 120);
            // mControlsListView.TileSize = new Size(size, mControlsListView.TileSize.Height);
        }
    }
}
