using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Otter.Plugins
{
    public class PluginAttribute : System.Attribute
    {
        #region Data
        private string mName = "";
        #endregion

        #region Properties
        public string Name
        {
            get { return mName; }
        }
        #endregion

        public PluginAttribute(string name)
        {
            mName = name;
        }
    }
}
