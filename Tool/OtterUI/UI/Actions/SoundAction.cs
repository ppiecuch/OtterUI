using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Drawing.Design;
using System.ComponentModel;

using Otter.Export;

namespace Otter.UI.Actions
{
    /// <summary>
    /// Notifies the parent view to play a sound
    /// </summary>
    public class SoundAction : Action
    {
        #region Data
        private int mSoundID = -1;
        private uint mVolume = 100;
        #endregion

        #region Properties
        /// <summary>
        /// Sound to play
        /// </summary>
        [TypeConverter(typeof(Otter.TypeConverters.SoundConverter))]
        [Editor(typeof(Otter.TypeEditors.UISoundEditor), typeof(UITypeEditor))]
        [XmlAttribute]
        public int Sound
        {
            get { return mSoundID; }
            set { mSoundID = value; }
        }

        /// <summary>
        /// Playback volume
        /// </summary>
        public uint Volume
        {
            get { return mVolume; }
            set 
            { 
                mVolume = value < 0 ? 0 : (value > 100 ? 100 : value); 
            }
        }
        #endregion

        /// <summary>
        /// Exports the action
        /// </summary>
        /// <param name="bw"></param>
        public override void Export(PlatformBinaryWriter bw)
        {
            FourCCStack fourCCStack = new FourCCStack(bw);
            fourCCStack.Push("SNDA");
            {
                bw.Write(this.Scene.GetUniqueSoundID(this.Sound));
                bw.Write((float)mVolume);
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
                UI.Resources.SoundInfo soundInfo = this.Scene.GetSoundInfo(mSoundID);

                if (soundInfo != null)
                    return "Play Sound : '" + soundInfo.Name + "'";
                else
                    return "Send Sound : '<nil>'";
            }

            return "Send Message ID : " + mSoundID;
        }

        /// <summary>
        /// Clones this action
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            SoundAction clone = new SoundAction();

            clone.Scene = this.Scene;
            clone.Sound = this.Sound;
            clone.Volume = this.Volume;

            return clone;
        }
    }
}
