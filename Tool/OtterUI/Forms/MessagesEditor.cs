using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

using Otter.UI;
using Otter.UI.Animation;
using Otter.UI.Actions;

namespace Otter.Forms
{
    public partial class MessagesEditor : Form
    {
        #region Data
        private GUIScene mScene = null;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the scene for which messages are being edited.
        /// </summary>
        public GUIScene Scene
        {
            get { return mScene; }
        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public MessagesEditor(GUIScene scene)
        {
            mScene = scene;
            InitializeComponent();
        }

        /// <summary>
        /// Called when the form has loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MessagesEditor_Load(object sender, EventArgs e)
        {
            PopulateDataGrid();
        }

        /// <summary>
        /// Populates the datagrid with glyph information
        /// </summary>
        private void PopulateDataGrid()
        {
            try
            {
                mMessagesDataGrid.Rows.Clear();

                foreach (Otter.UI.Message message in mScene.Messages)
                {
                    AddMessageToDataGrid(message);
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("EXCEPTION: " + ex);
            }
        }

        /// <summary>
        /// Adds a message to the datagrid
        /// </summary>
        /// <param name="message"></param>
        private void AddMessageToDataGrid(Otter.UI.Message message)
        {
            int index = mMessagesDataGrid.Rows.Add();

            // Prepare the Character Code cell
            DataGridViewTextBoxCell textCell = mMessagesDataGrid.Rows[index].Cells[0] as DataGridViewTextBoxCell;
            DataGridViewTextBoxCell descCell = mMessagesDataGrid.Rows[index].Cells[1] as DataGridViewTextBoxCell;

            textCell.Value = message.Text;
            descCell.Value = message.Description;

            // Finalize the row
            mMessagesDataGrid.Rows[index].Tag = message;
        }


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

        /// <summary>
        /// Called when the selection has changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mMessagesDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            bool hasSelection = (mMessagesDataGrid.SelectedRows.Count > 0);

            mRemoveButton.Enabled = hasSelection;
        }

        /// <summary>
        /// Adds a new message
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mAddButton_Click(object sender, EventArgs e)
        {
            string text = "MESSAGE_";
            int count = 0;
            int id = 0;
            // Get a unique ID
            foreach (Otter.UI.Message msg in mScene.Messages)
            {
                if (msg.ID >= id)
                    id = msg.ID + 1;
            }

            bool bUnique = false;
            while(!bUnique)
            {
                count++;
                bUnique = true;
                foreach (Otter.UI.Message msg in mScene.Messages)
                {
                    if (msg.Text == (text + count))
                    {
                        bUnique = false;
                        break;
                    }
                }
            }

            Otter.UI.Message message = new UI.Message();
            message.ID = id;
            message.Text = text + count;
            message.Description = "<enter description>";

            mScene.Messages.Add(message);
            AddMessageToDataGrid(message);
        }

        /// <summary>
        /// Removes the current selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mRemoveButton_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show(Otter.Properties.Resources.DELETE_MESSAGES,
                                                   Otter.Properties.Resources.WARNING,
                                                   MessageBoxButtons.YesNo,
                                                   MessageBoxIcon.Exclamation);
            if (res == DialogResult.No)
                return;

            foreach (DataGridViewRow row in mMessagesDataGrid.SelectedRows)
            {
                UI.Message message = ((UI.Message)row.Tag);


                mScene.RemoveMessage(message.ID);
                mMessagesDataGrid.Rows.Remove(row);
            }
        }

        /// <summary>
        /// Finds the current selection's references
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mFindReferencesButton_Click(object sender, EventArgs e)
        {

        }

        private void mMessagesDataGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {            
            DataGridViewRow row = mMessagesDataGrid.Rows[e.RowIndex];
            DataGridViewCell cell = row.Cells[e.ColumnIndex];

            // We need to validate the text
            if (row.Tag is UI.Message && e.ColumnIndex == 0)
            {
                UI.Message message = row.Tag as UI.Message;

                DataGridViewTextBoxCell textCell = cell as DataGridViewTextBoxCell;
                string text = textCell.Value as string;

                // Revert to previous if nothing was entered
                if(text == "" || text == null)
                {
                    textCell.Value = message.Text;
                    return;
                }

                // Bail out if nothing was changed.
                if (text == message.Text)
                {
                    return;
                }

                // Format it
                text = text.ToUpper();
                text = Regex.Replace(text, "[^A-Za-z0-9]", "_");
                text = Regex.Replace(text, "_+", "_");
                if (text.StartsWith("_"))
                    text = text.Remove(0);
                if (text.EndsWith("_"))
                    text = text.Remove(text.Length - 1);

                // Now see if there are any duplicates
                foreach (UI.Message msg in mScene.Messages)
                {
                    if (msg != message && msg.Text == text)
                    {
                        textCell.Value = message.Text;
                        return;
                    }
                }

                textCell.Value = text;
                message.Text = text;
            }
            else if (row.Tag is UI.Message && e.ColumnIndex == 1)
            {
                UI.Message message = row.Tag as UI.Message;

                DataGridViewTextBoxCell textCell = cell as DataGridViewTextBoxCell;
                string text = textCell.Value as string;

                // Revert to previous if nothing was entered
                if (text == "" || text == null)
                {
                    textCell.Value = message.Description;
                    return;
                }

                // Bail out if nothing was changed.
                if (text == message.Description)
                {
                    return;
                }

                textCell.Value = text;
                message.Description= text;
            }
        }
    }
}
