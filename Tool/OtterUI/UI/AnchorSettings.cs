using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace Otter.UI
{
    /// <summary>
    /// Control anchor flags
    /// NOTE:  THESE MUST MATCH IN TYPES.H
    /// </summary>
    [Flags()]
    public enum AnchorFlags
    {
        None = 0,

        Top = 1 << 0,
        Bottom = 1 << 1,
        Left = 1 << 2,
        Right = 1 << 3,

        TopRelative = 1 << 28,
        BottomRelative = 1 << 29,
        LeftRelative = 1 << 30,
        RightRelative = 1 << 31
    };

    public class AnchorData : ICloneable
    {	
		[XmlAttribute()]
		public float AbsoluteValue;

		[XmlAttribute()]
        public float RatioValue;

        /// <summary>
        /// Constructor
        /// </summary>
        public AnchorData()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public AnchorData(float absoluteValue, float ratioValue)
        {
            AbsoluteValue = absoluteValue;
            RatioValue = ratioValue;
        }

        /// <summary>
        /// Clones the anchor data
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new AnchorData(AbsoluteValue, RatioValue);
        }
    };
}
