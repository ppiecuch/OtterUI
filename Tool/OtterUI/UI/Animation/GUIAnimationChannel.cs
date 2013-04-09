using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Xml;
using System.Text;

namespace Otter.UI.Animation
{
    /// <summary>
    /// Animates a single GUI Control
    /// </summary>
    public class GUIAnimationChannel : ICloneable
    {
        #region Data
        private int mControlID = -1;
        private GUIControl mControl = null;

        // TODO : Create a KeyFrame Collection
        private List<KeyFrame> mKeyFrames = new List<KeyFrame>();
        #endregion

        #region Properties
        /// <summary>
        /// Gets / Sets the control count.  Not intended to be used aside from
        /// Xml (de)serialization.
        /// </summary>
        [XmlAttribute]
        [Browsable(false)]
        public int ControlID
        {
            get { return mControlID; }
            set { mControlID = value; }
        }

        /// <summary>
        /// Gets / Sets the animated control
        /// </summary>
        [ReadOnly(true)]
        [XmlIgnore]
        public GUIControl Control
        {
            get { return mControl; }
            set 
            { 
                mControl = value;

                if (mControl != null)
                    mControlID = mControl.ID;
                else
                    mControlID = -1;
            }
        }

        /// <summary>
        /// Gets / Sets the list of keyframes
        /// </summary>
        [XmlArrayItem(typeof(KeyFrame))]
        public List<KeyFrame> KeyFrames
        {
            get { return mKeyFrames; }
            set 
            {
                mKeyFrames = value;
            }
        }
        #endregion

        /// <summary>
        /// Retrieves a key frame by frame number
        /// 
        /// TODO : Need to move this logic into a KeyFrame collection
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public KeyFrame GetKeyFrame(int frameNumber)
        {
            foreach (KeyFrame keyFrame in mKeyFrames)
            {
                if (keyFrame.Frame == frameNumber)
                    return keyFrame;
            }

            return null;
        }

        /// <summary>
        /// Clones this channel in its entirety
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            GUIAnimationChannel channel = new GUIAnimationChannel();
            
            channel.mControlID = this.mControlID;            

            foreach(KeyFrame keyFrame in this.mKeyFrames)
                channel.mKeyFrames.Add((KeyFrame)keyFrame.Clone());

            return channel;
        }
    }
}
