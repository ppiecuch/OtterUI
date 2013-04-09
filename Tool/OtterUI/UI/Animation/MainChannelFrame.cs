using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Drawing.Design;

using Otter.Containers;
using Otter.Export;
using Otter.UI.Animation;
using Otter.UI.Actions;

namespace Otter.UI.Animation
{
    /// <summary>
    /// Contains a list of actions to execute on a particular frame
    /// </summary>
    public class MainChannelFrame : BaseFrame
    {
        #region Data
        private string mName = ""; // Name of the Main Channel Frame
        private NotifyingList<Otter.UI.Actions.Action> mActions = null;
        #endregion

        #region Properties
        /// <summary>
        /// Gets / Sets the parent animation
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public GUIAnimation Animation { get; set; }

        /// <summary>
        /// Gets / Sets the name of this frame
        /// </summary>
        [XmlAttribute]
        [Category("General")]
        public string Name
        {
            get { return mName; }
            set 
            { 
                mName = value;
                Validate();
            }
        }

        /// <summary>
        /// Gets / Sets the list of actions
        /// </summary>
        [XmlArrayItem(typeof(MessageAction))]
        [XmlArrayItem(typeof(SoundAction))]
        [Editor(typeof(Otter.TypeEditors.UIActionListEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(Otter.TypeConverters.ListConverter))]
        [Category("Behaviour")]
        public NotifyingList<Otter.UI.Actions.Action> Actions
        {
            get { return mActions; }
            set 
            { 
                mActions = value;
                mActions.OnItemAdded += new NotifyingList<UI.Actions.Action>.ListEventHandler(mActions_OnItemAdded);
                mActions.OnItemRemoved += new NotifyingList<UI.Actions.Action>.ListEventHandler(mActions_OnItemRemoved);

                Validate();
            }
        }
        #endregion

        #region Events : Notifying List
        void mActions_OnItemRemoved(object sender, Actions.Action item)
        {
            if(sender == Actions)
                Validate();
        }

        void mActions_OnItemAdded(object sender, Actions.Action item)
        {
            if(sender == Actions)
                Validate();
        }
        #endregion

        public MainChannelFrame()
        {
            Actions = new NotifyingList<Actions.Action>();
        }

        /// <summary>
        /// Adds / Removes this frame from the animation based on its properties.
        /// </summary>
        private void Validate()
        {
            if (Animation == null)
                return;

            if (Animation.MainChannelFrames.Contains(this) && Name == "" && Actions.Count == 0)
            {
                Animation.MainChannelFrames.Remove(this);
            }
            else if (!Animation.MainChannelFrames.Contains(this) && (Name != "" || Actions.Count > 0))
            {
                Animation.MainChannelFrames.Add(this);
            }
        }

        /// <summary>
        /// Exports the action list
        /// </summary>
        /// <param name="bw"></param>
        public void Export(PlatformBinaryWriter bw)
        {
            FourCCStack fourCCStack = new FourCCStack(bw);
            fourCCStack.Push("MCFR");
            {
                byte[] bytes = Utils.StringToBytes(mName, 64);
                bytes[63] = 0;
                bw.Write(bytes, 0, 64);

                bw.Write(this.Frame);
                bw.Write(this.Actions.Count);

                foreach (UI.Actions.Action action in this.Actions)
                {
                    action.Export(bw);
                }
            } 
            fourCCStack.Pop();
        }

        /// <summary>
        /// Clones this frame
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            MainChannelFrame frame = new MainChannelFrame();

            frame.Frame = this.Frame;
            frame.Name = this.Name;

            foreach(Otter.UI.Actions.Action action in this.mActions)
                frame.Actions.Add((Otter.UI.Actions.Action)action.Clone());

            return frame;
        }
    }
}
