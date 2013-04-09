using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Otter.UI.Animation
{
    /// <summary>
    /// Base "Frame" class, from which to build much more useful frame
    /// objects (ex, keyframes)
    /// </summary>
    public abstract class BaseFrame : ICloneable
    {
        #region Data
        private UInt32 mFrame = 0;
        #endregion

        #region Properties
        /// <summary>
        /// Gets / Sets the frame number that this keyframe applies to
        /// </summary>
        [XmlAttribute]
        [Browsable(false)]
        public UInt32 Frame
        {
            get { return mFrame; }
            set { mFrame = value; }
        }
        #endregion

        public abstract object Clone();
    }
}
