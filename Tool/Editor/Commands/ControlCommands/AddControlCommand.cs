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
    class AddControlCommand : ControlCommand
    {
        #region Data
        private GUIView mView = null;
        private GUIControl mParent = null;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="view"></param>
        /// <param name="control"></param>
        public AddControlCommand(GUIView view, GUIControl parent, GUIControl control)
        {
            mView = view;
            mParent = parent;
            Controls.Add(control);
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <returns>True if successful, false if otherwise.</returns>
        public override bool Execute()
        {
            GUIAnimation onActivate = mView.Animations["OnActivate"];
            
            if (onActivate != null)
                CreateKeyframes(onActivate, Controls[0]);
            
            mView.AddControl(mParent, Controls[0]);
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
            mView.RemoveControl(Controls[0]);
            return true;
        }

        /// <summary>
        /// Recursively create keyframes
        /// </summary>
        /// <param name="animation"></param>
        /// <param name="control"></param>
        private void CreateKeyframes(GUIAnimation animation, GUIControl control)
        {
            animation.CreateKeyFrame(control, 0);

            foreach (GUIControl child in control.Controls)
                CreateKeyframes(animation, child);
        }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Add Control: " + Controls[0].Name;
        }
    }
}
