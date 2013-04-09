using System;
using System.Collections.Generic;
using System.Text;

namespace Otter.Commands
{
    /// <summary>
    /// The Null Command doesn't do anything.  Useful for bookmarking or whatever.
    /// </summary>
    public class NullCommand : Command
    {
        private string mDesc;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="desc"></param>
        public NullCommand(string desc)
        {
            mDesc = desc;
        }

        /// <summary>
        /// Does nothing
        /// </summary>
        /// <returns></returns>
        public override bool Execute() { return true; }

        /// <summary>
        /// Does nothing
        /// </summary>
        /// <returns></returns>
        public override bool Undo() { return true; }
        
        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return mDesc; 
        }
    }
}
