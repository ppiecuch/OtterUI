using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Drawing.Design;
using System.ComponentModel;

using System.Drawing;

using Otter.Export;
using Otter.UI.Animation;
using Otter.Interface;
using Otter.TypeEditors;
using Otter.TypeConverters;
using Otter.UI.Resources;
using Otter.UI.Attributes;

namespace Otter.UI
{
    /// <summary>
    /// Sprite layout data
    /// </summary>
    public class SpriteLayout : ControlLayout
    {
        #region Data
        private Color mColor = Color.White;
        private float mSkew = 0.0f;
        #endregion

        #region Properties
        /// <summary>
        /// Gets / Sets the sprite's color
        /// </summary>
        [XmlIgnore]
        public Color Color
        {
            get { return mColor; }
            set { mColor = value; }
        }

        /// <summary>
        /// Gets / Sets the skew angle, in degrees
        /// </summary>
        public float Skew
        {
            get { return mSkew; }
            set { mSkew = value; }
        }

        /// <summary>
        /// Exports the color to xml
        /// </summary>
        [Browsable(false)]
        [XmlElement("Color")]
        public string ColorXML
        {
            get { return TypeDescriptor.GetConverter(typeof(Color)).ConvertToString(mColor); }
            set { mColor = (Color)TypeDescriptor.GetConverter(typeof(Color)).ConvertFrom(value); }
        }
        #endregion

        /// <summary>
        /// Clones the layout
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            SpriteLayout layout = new SpriteLayout();
            layout.SetFrom(this);

            layout.Color = this.Color;
            layout.Skew = this.Skew;

            return layout;
        }

        /// <summary>
        /// Exports the sprite layout
        /// </summary>
        /// <param name="bw"></param>
        public override void Export(PlatformBinaryWriter bw)
        {
            FourCCStack fourCCStack = new FourCCStack(bw);
            fourCCStack.Push("SPLT");
            {
                ExportBase(bw);

                bw.Write(Color);
                bw.Write(Skew);
            }
            fourCCStack.Pop();
        }

        /// <summary>
        /// GetHashCode override
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            SpriteLayout layout = (SpriteLayout)obj;
            return (base.Equals(obj) &&
                    layout.Color.Equals(this.Color));
        }
    }

    /// <summary>
    /// A simple sprite control.  Renders an image.
    /// </summary>
    public class GUISprite : GUIControl
    {
        #region Enums
        public enum FlipType
        {
            None,
            Horizontal,
            Vertical
        }

        #endregion

        #region Data
        private int mTextureID = -1;
        private FlipType mFlipType = FlipType.None;
        #endregion

        #region Properties
        /// <summary>
        /// Gets / Sets the texture count.
        /// </summary>
        [TypeConverter(typeof(TextureConverter))]
        [Editor(typeof(UITextureEditor), typeof(UITypeEditor))]
        [DisplayName("Texture")]
        public int TextureID
        {
            get 
            {
                return mTextureID; 
            }
            set
            {
                mTextureID = value;
            }
        }

        /// <summary>
        /// Returns the list of used texture IDs
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public override List<int> TextureIDs
        {
            get
            {
                List<int> list = new List<int>();
                list.Add(mTextureID);

                foreach (GUIControl control in Controls)
                {
                    List<int> childList = control.TextureIDs;

                    foreach (int texID in childList)
                        if (!list.Contains(texID))
                            list.Add(texID);
                }

                return list;
            }
        }

        /// <summary>
        /// Gets / Sets the sprite's color
        /// </summary>
        [XmlIgnore]
        [Category("Layout")]
        public Color Color
        {
            get { return (Layout as SpriteLayout).Color; }
            set { (Layout as SpriteLayout).Color = value; }
        }

        /// <summary>
        /// Gets / Sets the skew angle, in degrees
        /// </summary>
        [XmlIgnore]
        [Category("Layout")]
        public float Skew
        {
            get { return (Layout as SpriteLayout).Skew; }
            set { (Layout as SpriteLayout).Skew = value; }
        }

        /// <summary>
        /// Gets / Sets the flip flag for the sprite
        /// </summary>
        public FlipType Flip
        {
            get { return mFlipType; }
            set { mFlipType = value; }
        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public GUISprite()
        {
            Layout = new SpriteLayout();
            Name = "Sprite";
        }

        /// <summary>
        /// Fits the sprite to the same size as its current texture
        /// </summary>
        [Method("Fit to Texture")]
        public void FitToTexture()
        {
            TextureInfo info = Scene.GetTextureInfo(mTextureID);
            if (info == null || info.Width <= 0 || info.Height <= 0)
                return;

            this.Size = new SizeF(info.Width, info.Height);
        }

        /// <summary>
        /// Applies the provided keyframe to the control,
        /// overriding all needed properties
        /// </summary>
        /// <param name="frame"></param>
        public override void ApplyKeyFrame(KeyFrame startFrame, KeyFrame endFrame, float factor)
        {
            base.ApplyKeyFrame(startFrame, endFrame, factor);

            SpriteLayout startLayout = startFrame != null ? startFrame.Layout as SpriteLayout : null;
            SpriteLayout endLayout = endFrame != null ? endFrame.Layout as SpriteLayout : null;

            if (startLayout == null && endLayout == null)
                return;

            int r, g, b, a;
            float skew;

            if (endLayout == null)
            {
                r = startLayout.Color.R;
                g = startLayout.Color.G;
                b = startLayout.Color.B;
                a = startLayout.Color.A;

                skew = startLayout.Skew;
            }
            else
            {
                r = (int)((float)startLayout.Color.R * (1.0f - factor) + (float)endLayout.Color.R * factor);
                g = (int)((float)startLayout.Color.G * (1.0f - factor) + (float)endLayout.Color.G * factor);
                b = (int)((float)startLayout.Color.B * (1.0f - factor) + (float)endLayout.Color.B * factor);
                a = (int)((float)startLayout.Color.A * (1.0f - factor) + (float)endLayout.Color.A * factor);

                skew = startLayout.Skew * (1.0f - factor) + endLayout.Skew * factor;
            }

            this.Color = Color.FromArgb(a, r, g, b);
            this.Skew = skew;
        }

        /// <summary>
        /// Draws the sprite
        /// </summary>
        public override void Draw(Otter.Interface.Graphics graphics)
        {
            TextureInfo info = Scene.GetTextureInfo(mTextureID);
            int texID = (info != null) ? info.TextureID : -1;

            switch(Flip)
            {
                case FlipType.None:
                    {
                        graphics.DrawRectangle(texID, 0.0f, 0.0f, Layout.Size.Width, Layout.Size.Height, 0.0f, 0.0f, 1.0f, 1.0f, ((SpriteLayout)Layout).Color.ToArgb(), Skew, InheritedMask);
                        break;
                    }
                case FlipType.Vertical:
                    {
                        graphics.DrawRectangle(texID, 0.0f, 0.0f, Layout.Size.Width, Layout.Size.Height, 0.0f, 1.0f, 1.0f, 0.0f, ((SpriteLayout)Layout).Color.ToArgb(), Skew, InheritedMask);
                        break;
                    }
                case FlipType.Horizontal:
                    {
                        graphics.DrawRectangle(texID, 0.0f, 0.0f, Layout.Size.Width, Layout.Size.Height, 1.0f, 0.0f, 0.0f, 1.0f, ((SpriteLayout)Layout).Color.ToArgb(), -Skew, InheritedMask);
                        break;
                    }
            }

            base.Draw(graphics);
        }

        /// <summary>
        /// Exports the GUI Sprite to disk
        /// </summary>
        /// <param name="bw"></param>
        public override void Export(PlatformBinaryWriter bw)
        {
            FourCCStack fourCCStack = new FourCCStack(bw);
            fourCCStack.Push("GSPR");
            {
                base.Export(bw);

                bw.Write(Scene.GetUniqueTextureID(mTextureID));
                bw.Write(Color.White.ToArgb());
                bw.Write(0.0f); // Skew
                bw.Write((UInt32)mFlipType);

                base.ExportControls(bw);
            }

            fourCCStack.Pop();
        }

        /// <summary>
        /// Clones (deep copies) the GUI Sprite
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            GUISprite ret = new GUISprite();
            if (ret == null) 
                throw new OutOfMemoryException("GUISprite.Clone() failed its allocation.");

            // GUIControl level copy
            ret.ID              = this.ID;
            ret.Parent          = this.Parent;
            ret.Name            = this.Name;
            ret.Layout          = this.Layout.Clone() as SpriteLayout;
            ret.Scene           = this.Scene;
            ret.AnchorFlags     = this.AnchorFlags;
            ret.Mask            = this.Mask;

            // GUISprite level copy
            ret.TextureID       = this.TextureID;
            ret.Skew            = this.Skew;
            ret.Flip            = this.Flip;

            return ret;
        }
    }
}
