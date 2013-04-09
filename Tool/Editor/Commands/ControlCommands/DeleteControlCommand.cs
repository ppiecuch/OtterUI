using System;
using System.Collections.Generic;
using System.Text;

using Otter.Commands;
using Otter.UI;
using Otter.UI.Animation;

namespace Otter.Editor.Commands.ControlCommands
{
    /// <summary>
    /// Command to add a control to a parent view 
    /// </summary>
    class DeleteControlCommand : ControlCommand
    {
        #region Data
        private GUIControl mParent = null;
        private GUIView mView = null;
        private int mIndex = 0;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="view"></param>
        /// <param name="control"></param>
        public DeleteControlCommand(GUIView view, GUIControl control)
        {
            mView = view;
            Controls.Add(control);
            mParent = Controls[0].Parent;
            mIndex = mView.Controls.IndexOf(Controls[0]);
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <returns>True if successful, false if otherwise.</returns>
        public override bool Execute()
        {
            mView.RemoveControl(Controls[0]);
            return true;
        }

        /// <summary>
        /// Undoes the command.  The undo must completely undo the command,
        /// such that the Execute() .. Undo() .. Execute() would start and end
        /// in the same state.
        /// </summary>
        /// <returns></returns>
        public override bool Undo()
        {
            mView.AddControl(mParent, Controls[0], mIndex);
            return true;
        }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Delete Control: " + Controls[0].Name;
        }
    }
}
