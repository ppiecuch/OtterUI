using System;
using System.Collections.Generic;
using System.Text;

using Otter.Commands;
using Otter.UI;

namespace Otter.Editor.Commands.ViewCommands
{
    /// <summary>
    /// Base command that operates on controls.
    /// </summary>
    public abstract class ViewCommand : Command
    {
        #region Data
        protected GUIView mView = null;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the view associated with this command
        /// </summary>
        public GUIView View
        {
            get { return mView; }
        }
        #endregion
    }
}
