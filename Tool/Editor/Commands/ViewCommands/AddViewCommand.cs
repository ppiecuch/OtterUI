using System;
using System.Collections.Generic;
using System.Text;

using Otter.Commands;
using Otter.UI;

namespace Otter.Editor.Commands.ViewCommands
{
    /// <summary>
    /// Command to add a view to a scene
    /// </summary>
    public class AddViewCommand : ViewCommand
    {
        #region Data
        private GUIScene mScene = null;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="view"></param>
        public AddViewCommand(GUIScene scene, GUIView view)
        {
            mScene = scene;
            mView = view;
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <returns>True if successful, false if otherwise.</returns>
        public override bool Execute()
        {
            mScene.AddView(mView);
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
            mScene.RemoveView(mView);
            return true;
        }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Add View: " + mView.Name;
        }
    }
}
