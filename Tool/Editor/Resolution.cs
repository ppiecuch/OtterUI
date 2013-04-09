//
// Copyright (c) Aonyx Software, LLC
// All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Otter.Editor
{
    public class Resolution
    {
        #region Data
        private int mWidth = 0;
        private int mHeight = 0;
        #endregion

        /// <summary>
        /// Gets / Sets the resolution width
        /// </summary>
        [XmlAttribute]
        public int Width
        {
            get { return mWidth; }
            set { mWidth = value; }
        }

        /// <summary>
        /// Gets / Sets the resolution height
        /// </summary>
        [XmlAttribute]
        public int Height
        {
            get { return mHeight; }
            set { mHeight = value; }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Resolution()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        public Resolution(int w, int h)
        {
            mWidth = w;
            mHeight = h;
        }
        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return mWidth + " x " + mHeight;
        }

        /// <summary>
        /// Equals Override
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            Resolution other = obj as Resolution;
            if (other != null)
                return other.Width == Width && other.Height == Height;

            return base.Equals(obj);
        }

        /// <summary>
        /// GetHashCode override
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
