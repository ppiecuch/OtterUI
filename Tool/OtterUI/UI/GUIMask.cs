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

namespace Otter.UI
{
    /// <summary>
    /// Mask layout data
    /// </summary>
    public class MaskLayout : ControlLayout
    {
        #region Data
        private float mSkew = 0.0f;
        #endregion

        #region Properties
        /// <summary>
        /// Gets / Sets the skew angle, in degrees
        /// </summary>
        public float Skew
        {
            get { return mSkew; }
            set { mSkew = value; }
        }
        #endregion

        /// <summary>
        /// Clones the layout
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            MaskLayout layout = new MaskLayout();
            layout.SetFrom(this);

            layout.Skew = this.Skew;

            return layout;
        }

        /// <summary>
        /// Exports the mask layout
        /// </summary>
        /// <param name="bw"></param>
        public override void Export(PlatformBinaryWriter bw)
        {
            FourCCStack fourCCStack = new FourCCStack(bw);
            fourCCStack.Push("SPLT");
            {
                ExportBase(bw);

                bw.Write(Skew);
            }
            fourCCStack.Pop();
        }
    }

    /// <summary>
    /// A simple mask control.  Renders an image.
    /// </summary>
    public class GUIMask : GUIControl
    {
        #region Enums

        #endregion

        #region Data
        private int mTextureID = -1;
        private GUISprite.FlipType mFlipType = GUISprite.FlipType.None;
        private Color mColor = Color.FromArgb(0x460000FF);
        #endregion

        #region Hidden Properties
        [Browsable(false)]
        [XmlIgnore]
        public new int Mask
        {
            get { return base.Mask; }
            set { base.Mask = value; }
        }
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
        /// Gets / Sets the skew angle, in degrees
        /// </summary>
        [XmlIgnore]
        [Category("Layout")]
        public float Skew
        {
            get { return (Layout as MaskLayout).Skew; }
            set { (Layout as MaskLayout).Skew = value; }
        }

        /// <summary>
        /// Gets / Sets the flip flag for the mask
        /// </summary>
        public GUISprite.FlipType Flip
        {
            get { return mFlipType; }
            set { mFlipType = value; }
        }

        /// <summary>
        /// Gets / Sets the mask's in-editor placeholder color
        /// </summary>
        [XmlIgnore]
        [Category("Layout")]
        [DisplayName("Preview Color")]
        public Color Color
        {
            get { return mColor; }
            set { mColor = value; }
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
        /// Constructor
        /// </summary>
        public GUIMask()
        {
            Layout = new MaskLayout();
            Name = "Mask";
        }

        /// <summary>
        /// Applies the provided keyframe to the control,
        /// overriding all needed properties
        /// </summary>
        /// <param name="frame"></param>
        public override void ApplyKeyFrame(KeyFrame startFrame, KeyFrame endFrame, float factor)
        {
            base.ApplyKeyFrame(startFrame, endFrame, factor);

            MaskLayout startLayout = startFrame != null ? startFrame.Layout as MaskLayout : null;
            MaskLayout endLayout = endFrame != null ? endFrame.Layout as MaskLayout : null;

            if (startLayout == null && endLayout == null)
                return;

            float skew;

            if (endLayout == null)
            {
                skew = startLayout.Skew;
            }
            else
            {
                skew = startLayout.Skew * (1.0f - factor) + endLayout.Skew * factor;
            }

            this.Skew = skew;
        }

        /// <summary>
        /// Draws the mask
        /// </summary>
        public override void Draw(Otter.Interface.Graphics graphics)
        {
            if (Hidden)
                return;

            //draw a placeholder sprite to show the mask's position
            TextureInfo info = Scene.GetTextureInfo(mTextureID);
            int texID = (info != null) ? info.TextureID : -1;

            switch (Flip)
            {
                case GUISprite.FlipType.None:
                    {
                        graphics.DrawRectangle(texID, 0.0f, 0.0f, Layout.Size.Width, Layout.Size.Height, 0.0f, 0.0f, 1.0f, 1.0f, Color.ToArgb(), Skew, -1);
                        break;
                    }
                case GUISprite.FlipType.Vertical:
                    {
                        graphics.DrawRectangle(texID, 0.0f, 0.0f, Layout.Size.Width, Layout.Size.Height, 0.0f, 1.0f, 1.0f, 0.0f, Color.ToArgb(), Skew, -1);
                        break;
                    }
                case GUISprite.FlipType.Horizontal:
                    {
                        graphics.DrawRectangle(texID, 0.0f, 0.0f, Layout.Size.Width, Layout.Size.Height, 1.0f, 0.0f, 0.0f, 1.0f, Color.ToArgb(), -Skew, -1);
                        break;
                    }
            }
        }

        public void DrawMask(Otter.Interface.Graphics graphics)
        {            
            //set our stored matrix as the stencil matrix
            graphics.SetStencilMatrix(this.FullTransform.Entries);

            TextureInfo info = Scene.GetTextureInfo(mTextureID);
            int texID = (info != null) ? info.TextureID : -1;

            switch (Flip)
            {
                case GUISprite.FlipType.None:
                    {
                        graphics.DrawRectangleStencil(texID, 0.0f, 0.0f, Layout.Size.Width, Layout.Size.Height, 0.0f, 0.0f, 1.0f, 1.0f, Color.White.ToArgb(), Skew, ID);
                        break;
                    }
                case GUISprite.FlipType.Vertical:
                    {
                        graphics.DrawRectangleStencil(texID, 0.0f, 0.0f, Layout.Size.Width, Layout.Size.Height, 0.0f, 1.0f, 1.0f, 0.0f, Color.White.ToArgb(), Skew, ID);
                        break;
                    }
                case GUISprite.FlipType.Horizontal:
                    {
                        graphics.DrawRectangleStencil(texID, 0.0f, 0.0f, Layout.Size.Width, Layout.Size.Height, 1.0f, 0.0f, 0.0f, 1.0f, Color.White.ToArgb(), -Skew, ID);
                        break;
                    }
            }
        }

        /// <summary>
        /// Exports the GUI Mask to disk
        /// </summary>
        /// <param name="bw"></param>
        public override void Export(PlatformBinaryWriter bw)
        {
            FourCCStack fourCCStack = new FourCCStack(bw);
            fourCCStack.Push("GMSK");
            {
                base.Export(bw);

                bw.Write(Scene.GetUniqueTextureID(mTextureID));
                bw.Write(0.0f); // Skew
                bw.Write((UInt32)mFlipType);
            }

            fourCCStack.Pop();
        }

        /// <summary>
        /// Clones (deep copies) the GUI Mask
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            GUIMask ret = new GUIMask();
            if (ret == null)
                throw new OutOfMemoryException("GUIMask.Clone() failed its allocation.");

            // GUIControl level copy
            ret.ID = this.ID;
            ret.Parent = this.Parent;
            ret.Name = this.Name;
            ret.Layout = this.Layout.Clone() as MaskLayout;
            ret.Scene = this.Scene;
            ret.AnchorFlags = this.AnchorFlags;
            ret.Mask = this.Mask;

            // GUIMask level copy
            ret.TextureID = this.TextureID;
            ret.Skew = this.Skew;
            ret.Flip = this.Flip;

            return ret;
        }
    }
}
