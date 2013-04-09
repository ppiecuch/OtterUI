using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using Otter.Commands;
using Otter.UI;

namespace Otter.Editor.Commands.ControlCommands
{
    /// <summary>
    /// Command to move a control (alter its position)
    /// </summary>
    class MoveControlCommand : ControlCommand
    {
        #region Data
        private PointF mDelta = PointF.Empty;
        #endregion

        #region Properties
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public MoveControlCommand(List<GUIControl> controls, PointF delta)
        {
            Controls = new List<GUIControl>(controls);
            mDelta = delta;
        }

        /// <summary>
        /// Executes the command
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            foreach(GUIControl control in Controls)
                control.Location = new PointF(control.Location.X + mDelta.X, control.Location.Y + mDelta.Y);

            return true;
        }

        /// <summary>
        /// Undoes the command
        /// </summary>
        /// <returns></returns>
        public override bool Undo()
        {
            foreach (GUIControl control in Controls)
                control.Location = new PointF(control.Location.X - mDelta.X, control.Location.Y - mDelta.Y);
            return true;
        }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Move Controls: " + Controls.Count;
        }
    }
}
