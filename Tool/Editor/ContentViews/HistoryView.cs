using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Otter.Commands;

using WeifenLuo.WinFormsUI.Docking;

namespace Otter.Editor.ContentViews
{
    public partial class HistoryView : DockContent
    {
        #region Data
        private CommandManager mCommandManager = null;
        #endregion

        #region Properties
        public CommandManager CommandManager
        {
            get
            {
                return mCommandManager;
            }
            set
            {
                if (mCommandManager == value)
                    return;

                if (mCommandManager != null)
                {
                    mCommandManager.OnCommandAdded -= new CommandManager.CommandDelegate(mCommandManager_OnCommandAdded);
                    mCommandManager.OnCommandRemoved -= new CommandManager.CommandDelegate(mCommandManager_OnCommandRemoved);
                }

                mCommandManager = value;
                if (mCommandManager != null)
                {
                    mCommandManager.OnCommandAdded += new CommandManager.CommandDelegate(mCommandManager_OnCommandAdded);
                    mCommandManager.OnCommandRemoved += new CommandManager.CommandDelegate(mCommandManager_OnCommandRemoved);
                }

                RebuildHistoryView();
            }
        }

        /// <summary>
        /// Called whenever a new command is added to the command manager.
        /// We need to update our list to include the new command
        /// </summary>
        /// <param name="command"></param>
        void mCommandManager_OnCommandAdded(Command command)
        {
            // Ensure no selection
            mHistoryListBox.SelectedIndex = -1;

            while (mCommandManager.Commands.Count <= mHistoryListBox.Items.Count)
                mHistoryListBox.Items.RemoveAt(mHistoryListBox.Items.Count - 1);
            
            mHistoryListBox.Items.Add(command);
        }

        /// <summary>
        /// Called whenever a command is removed from the command manager.
        /// We need to update our list to exclude the new command.
        /// </summary>
        /// <param name="command"></param>
        void mCommandManager_OnCommandRemoved(Command command)
        {
            mHistoryListBox.Items.Remove(command);
        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public HistoryView()
        {
            InitializeComponent();
            
            this.HideOnClose = true;
        }

        /// <summary>
        /// Rebuilds the history from scratch
        /// </summary> 
        public void RebuildHistoryView()
        {
            mHistoryListBox.Items.Clear();

            if (mCommandManager != null)
            {
                foreach (Command command in mCommandManager.Commands)
                    mHistoryListBox.Items.Add(command);
            }
        }

        /// <summary>
        /// Called whenever the selected history item index has changed.
        /// We need to traverse up and down the history list to make sure
        /// the appropriate items are executed/undone
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mHistoryListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mHistoryListBox.SelectedIndex != -1)
            {
                int index = mHistoryListBox.SelectedIndex;
                while (mCommandManager.CurrentCommandIndex != index)
                {
                    if (mCommandManager.CurrentCommandIndex > index)
                    {
                        if (!mCommandManager.Undo())
                            return;
                    }
                    else
                    {
                        if (!mCommandManager.Redo())
                            return;
                    }
                }
            }
        }

        /// <summary>
        /// Undoes a step
        /// </summary>
        public void Undo()
        {
            if (mHistoryListBox.SelectedIndex == -1)
            {
                // Count - 1 is the last step.  We want to go one back if possible.
                mHistoryListBox.SelectedIndex = System.Math.Max(mHistoryListBox.Items.Count - 2, 0);
            }
            else if (mHistoryListBox.SelectedIndex > 0)
                mHistoryListBox.SelectedIndex--;
        }

        /// <summary>
        /// Redoes a step
        /// </summary>
        public void Redo()
        {
            if (mHistoryListBox.SelectedIndex == -1)
            {
                // 0 is the start.  Skip it if possible.
                mHistoryListBox.SelectedIndex = System.Math.Min(mHistoryListBox.Items.Count - 1, 1);
            }
            else if (mHistoryListBox.SelectedIndex < (mHistoryListBox.Items.Count - 1))
                mHistoryListBox.SelectedIndex++;
        }
    }
}
