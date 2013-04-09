using System;
using System.Collections.Generic;
using System.Text;

namespace Otter.Commands
{
    /// <summary>
    /// Represents a single command that can be executed and undone.
    /// </summary>
    public abstract class Command
    {
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <returns>True if successful, false if otherwise.</returns>
        public abstract bool Execute();

        /// <summary>
        /// Undoes the command.  The undo must completely undo the command,
        /// such that the Execute() .. Undo() .. Execute() would start and end
        /// in the same state.
        /// </summary>
        /// <returns></returns>
        public abstract bool Undo();
    }
}
