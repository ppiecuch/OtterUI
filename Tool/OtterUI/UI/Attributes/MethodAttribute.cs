using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Otter.UI.Attributes
{
    /// <summary>
    /// Describes a control method that can be exposed in the Editor
    /// </summary>
    public class MethodAttribute : System.Attribute
    {
#region Data
        private string mDescription;
#endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        public MethodAttribute(string description)
        {
            mDescription = description;
        }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return mDescription;
        }
    }
}
