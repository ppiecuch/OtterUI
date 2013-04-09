using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using Otter.Commands;
using Otter.UI;

namespace Otter.Editor.Commands.ControlCommands
{
    /// <summary>
    /// Command to resize a control
    /// </summary>
    public class ResizeControlCommand : ControlCommand
    {
        #region Data
        private PointF mSourceCenter = PointF.Empty;
        private SizeF mSourceSize = SizeF.Empty;
        private PointF mDestCenter = PointF.Empty;
        private SizeF mDestSize = SizeF.Empty;
        #endregion

        #region Properties
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public ResizeControlCommand(GUIControl control, PointF srcCenter, SizeF srcSize, PointF dstCenter, SizeF dstSize)
        {
            Controls.Add(control);

            mSourceCenter = srcCenter;
            mSourceSize = srcSize;
            mDestCenter = dstCenter;
            mDestSize = dstSize;
        }

        /// <summary>
        /// Executes the command
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            Controls[0].Center = mDestCenter;
            Controls[0].Size = mDestSize;
            return true;
        }

        /// <summary>
        /// Undoes the command
        /// </summary>
        /// <returns></returns>
        public override bool Undo()
        {
            Controls[0].Center = mSourceCenter;
            Controls[0].Size = mSourceSize;
            return true;
        }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Resize Control: " + Controls[0].Name;
        }
    }
}
