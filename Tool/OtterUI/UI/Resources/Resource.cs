using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Otter.UI.Resources
{
    public class Resource
    {
        #region Events and Delegates
        public delegate void ResourceEventHandler(object sender);
        public event ResourceEventHandler OnResourceUpdated = null;
        #endregion

        #region Data
        private int mID = -1;
        #endregion

        #region Properties
        /// <summary>
        /// DeviceTexture Info ID
        /// </summary>
        [XmlAttribute]
        public int ID
        {
            get { return mID; }
            set { mID = value; }
        }
        #endregion

        /// <summary>
        /// Loads the resource
        /// </summary>
        /// <returns></returns>
        public virtual bool Load()
        {
            return false;
        }

        /// <summary>
        /// Unloads the resource
        /// </summary>
        /// <returns></returns>
        public virtual bool Unload()
        {
            return false;
        }

        /// <summary>
        /// Notifies that the resource has been updated.
        /// </summary>
        protected void NotifyResourceUpdated()
        {
            if (OnResourceUpdated != null)
                OnResourceUpdated(this);
        }
    }
}
