using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Serialization;

using Otter.Export;

namespace Otter.UI.Actions
{
    /// <summary>
    /// Messages the parent view with a string
    /// </summary>
    public class MessageAction : Action
    {
        #region Data
        private int mMessageID = -1;
        #endregion

        #region Properties
        /// <summary>
        /// Message to send
        /// </summary>
        [TypeConverter(typeof(Otter.TypeConverters.MessageConverter))]
        [Editor(typeof(Otter.TypeEditors.UIMessageEditor), typeof(UITypeEditor))]
        [XmlAttribute]
        public int Message
        {
            get { return mMessageID; }
            set { mMessageID = value; }
        }
        #endregion

        /// <summary>
        /// Exports the action
        /// </summary>
        /// <param name="bw"></param>
        public override void Export(PlatformBinaryWriter bw)
        {
            FourCCStack fourCCStack = new FourCCStack(bw);
            fourCCStack.Push("MSGA");
            {
                bw.Write(this.Scene.GetUniqueMessageID(this.Message));
            }
            fourCCStack.Pop();
        }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this.Scene != null)
            {
                UI.Message message = this.Scene.GetMessage(mMessageID);

                if (message != null)
                    return "Send Message : '" + message + "'";
                else
                    return "Send Message : '<nil>'";
            }

            return "Send Message ID : " + mMessageID;
        }

        /// <summary>
        /// Clones this action
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            MessageAction clone = new MessageAction();

            clone.Scene = this.Scene;
            clone.Message = this.Message;

            return clone;
        }
    }
}
