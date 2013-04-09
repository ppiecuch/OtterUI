using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Otter.UI;

namespace Otter.Forms
{
    /// <summary>
    /// Form to edit the list of actions
    /// </summary>
    public partial class ActionsEditor : Form
    {
        #region Data
        private GUIScene mScene = null;
        private List<Otter.UI.Actions.Action> mActions = new List<UI.Actions.Action>();
        #endregion

        #region Properties
        /// <summary>
        /// Gets / Sets the list of actions
        /// </summary>
        public List<Otter.UI.Actions.Action> Actions
        {
            get { return mActions; }
            set 
            { 
                mActions = new List<UI.Actions.Action>(value);

                mItemsListView.Items.Clear();
                foreach (UI.Actions.Action action in mActions)
                {
                    ListViewItem item = new ListViewItem(action.ToString());

                    if (action is UI.Actions.MessageAction)
                        item.ImageIndex = 0;
                    else if (action is UI.Actions.SoundAction)
                        item.ImageIndex = 1;

                    item.Tag = action;
                    mItemsListView.Items.Add(item);
                }
            }
        }
        #endregion

        #region Event Handlers : mActionTypesComboBox
        /// <summary>
        /// Called when the selected index has changed.  If it's anything but 0, create the appropriate
        /// action
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mActionTypesComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Otter.UI.Actions.Action newAction = null;
            switch (mActionTypesComboBox.SelectedIndex)
            {
                // Do nothing
                case 0:
                    {
                        return;
                    }
                // Send Message
                case 1:
                    {
                        newAction = new Otter.UI.Actions.MessageAction();
                        break;
                    }
                // Play Sound
                case 2:
                    {
                        newAction = new Otter.UI.Actions.SoundAction();
                        break;
                    }
            }

            newAction.Scene = mScene;
            mActions.Add(newAction);

            Actions = mActions;
            mActionTypesComboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Removes the selected item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mRemoveButton_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in mItemsListView.SelectedItems)
            {
                Otter.UI.Actions.Action action = item.Tag as Otter.UI.Actions.Action;
                mActions.Remove(action);
            }

            Actions = new List<UI.Actions.Action>(mActions);
        }
        #endregion

        #region Event Handlers : mItemsListView
        /// <summary>
        /// Items List Box's index has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mItemsListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mItemsListView.SelectedItems.Count > 0)
            {
                List<UI.Actions.Action> actions = new List<UI.Actions.Action>();
                foreach (ListViewItem item in mItemsListView.SelectedItems)
                    actions.Add(item.Tag as UI.Actions.Action);

                mPropertyGrid.SelectedObjects = actions.ToArray();
                mRemoveButton.Enabled = true;
            }
            else
            {
                mPropertyGrid.SelectedObjects = null;
                mRemoveButton.Enabled = false;
            }
        }
        #endregion

        #region Event Handlers : mPropertyGrid
        /// <summary>
        /// Called when a value has changed.  Update the listbox to reflect the changes
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        private void mPropertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            foreach (ListViewItem item in mItemsListView.Items)
                item.Text = ((UI.Actions.Action)item.Tag).ToString();
        }
        #endregion

        #region Event Handlers : Actions Editor
        /// <summary>
        /// "OK" button has been clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mOKButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public ActionsEditor(GUIScene scene)
        {
            mScene = scene;

            InitializeComponent();

            mActionTypesComboBox.Items.Clear();
            mActionTypesComboBox.Items.Add("Add Action...");
            mActionTypesComboBox.Items.Add("Send Message");
            mActionTypesComboBox.Items.Add("Play Sound");

            mActionTypesComboBox.SelectedIndex = 0;
        }
    }
}
