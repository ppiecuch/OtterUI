using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

using Otter.Commands;

namespace Otter.Editor.Commands
{
    /// <summary>
    /// Catch-all Property Changed command
    /// </summary>
    public class PropertyChangedCommand : Command
    {
        #region Data
        private object mObject = null;
        private PropertyDescriptor mDescriptor = null;
        private object mOldValue = null;
        private object mNewValue = null;
        #endregion

        #region Properties
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public PropertyChangedCommand(object obj, PropertyDescriptor descriptor, object oldValue, object newValue)
        {
            mObject = obj;
            mDescriptor = descriptor;
            mOldValue = oldValue;
            mNewValue = newValue;
        }

        /// <summary>
        /// Executes the command
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            mDescriptor.SetValue(mObject, mNewValue);
            return true;
        }

        /// <summary>
        /// Undoes the command
        /// </summary>
        /// <returns></returns>
        public override bool Undo()
        {
            mDescriptor.SetValue(mObject, mOldValue);
            return true;
        }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Property Changed: " + mDescriptor.Name;
        }
    }
}
