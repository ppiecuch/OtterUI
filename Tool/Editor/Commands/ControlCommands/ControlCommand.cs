using System;
using System.Collections.Generic;
using System.Text;

using Otter.Commands;
using Otter.UI;

namespace Otter.Editor.Commands.ControlCommands
{
    /// <summary>
    /// Base command that operates on controls.
    /// </summary>
    public abstract class ControlCommand : Command
    {
        #region Data
        private List<GUIControl> mControls = new List<GUIControl>();
        #endregion

        #region Properties
        /// <summary>
        /// Gets the control(s) associated with this command
        /// </summary>
        public List<GUIControl> Controls
        {
            get { return mControls; }
            set { mControls = value; }
        }
        #endregion
    }
}
