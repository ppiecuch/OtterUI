using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Otter.UI
{
    /// <summary>
    /// Represents a generic message that can be sent to a view
    /// </summary>
    public class Message
    {
        #region Data
        private int mID = 0;
        private string mText = "";
        private string mDescription = "";
        #endregion

        #region Properties
        /// <summary>
        /// Gets / Sets the ID of the message
        /// </summary>
        [XmlAttribute]
        public int ID
        {
            get { return mID; }
            set { mID = value; }
        }

        /// <summary>
        /// Gets / Sets the text of the message
        /// </summary>
        [XmlAttribute]
        public string Text
        {
            get { return mText; }
            set { mText = value; }
        }

        /// <summary>
        /// Gets / Sets the message description
        /// </summary>
        [XmlText]
        public string Description
        {
            get { return mDescription; }
            set { mDescription = value; }
        }
        #endregion

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Text;
        }
    }
}
