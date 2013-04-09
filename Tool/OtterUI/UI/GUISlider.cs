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
    /// Slider layout data
    /// </summary>
    public class SliderLayout : ControlLayout
    {
        #region Data
        private Color mColor = Color.White;
        #endregion

        #region Properties
        /// <summary>
        /// Gets / Sets the Slider's color
        /// </summary>
        [XmlIgnore]
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
        /// Clones the layout
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            SliderLayout layout = new SliderLayout();
            layout.SetFrom(this);

            layout.Color = this.Color;

            return layout;
        }

        /// <summary>
        /// Exports the Slider layout
        /// </summary>
        /// <param name="bw"></param>
        public override void Export(PlatformBinaryWriter bw)
        {
            FourCCStack fourCCStack = new FourCCStack(bw);
            fourCCStack.Push("SLLT");
            {
                ExportBase(bw);

                bw.Write(Color);
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
            SliderLayout layout = (SliderLayout)obj;
            return (base.Equals(obj) &&
                    layout.Color.Equals(this.Color));
        }
    }

    /// <summary>
    /// A simple Slider control.  Renders an image.
    /// </summary>
    public class GUISlider : GUIControl
    {
        #region Data
        private int mStartTextureID = -1;
        private int mMiddleTextureID = -1;
        private int mEndTextureID = -1;
        private int mThumbTextureID = -1;

        private int mMin = 0;
        private int mMax = 0;
        private int mStep = 0;
        #endregion

        #region Properties
        /// <summary>
        /// Gets / Sets the Slider's color
        /// </summary>
        [XmlIgnore]
        [Category("Layout")]
        public Color Color
        {
            get { return (Layout as SliderLayout).Color; }
            set { (Layout as SliderLayout).Color = value; }
        }

        /// <summary>
        /// Gets / Sets the thumb width
        /// </summary>
        [DisplayName("Thumb Width")]
        [Category("Appearance")]
        public uint ThumbWidth { get; set; }

        /// <summary>
        /// Gets / Sets the thumb height
        /// </summary>
        [DisplayName("Thumb Height")]
        [Category("Appearance")]
        public uint ThumbHeight { get; set; }

        /// <summary>
        /// Gets / Sets the start texture
        /// </summary>
        [TypeConverter(typeof(TextureConverter))]
        [Editor(typeof(UITextureEditor), typeof(UITypeEditor))]
        [DisplayName("Cap Start Texture")]
        [Category("Appearance")]
        public int StartTexture
        {
            get { return mStartTextureID; }
            set { mStartTextureID = value; }
        }

        /// <summary>
        /// Gets / Sets the middle texture
        /// </summary>
        [TypeConverter(typeof(TextureConverter))]
        [Editor(typeof(UITextureEditor), typeof(UITypeEditor))]
        [DisplayName("Middle Texture")]
        [Category("Appearance")]
        public int MiddleTexture
        {
            get { return mMiddleTextureID; }
            set { mMiddleTextureID = value; }
        }

        /// <summary>
        /// Gets / Sets the end texture
        /// </summary>
        [TypeConverter(typeof(TextureConverter))]
        [Editor(typeof(UITextureEditor), typeof(UITypeEditor))]
        [DisplayName("Cap End Texture")]
        [Category("Appearance")]
        public int EndTexture
        {
            get { return mEndTextureID; }
            set { mEndTextureID = value; }
        }

        /// <summary>
        /// Gets / Sets the thumb texture
        /// </summary>
        [TypeConverter(typeof(TextureConverter))]
        [Editor(typeof(UITextureEditor), typeof(UITypeEditor))]
        [DisplayName("Thumb Texture")]
        [Category("Appearance")]
        public int ThumbTexture
        {
            get { return mThumbTextureID; }
            set { mThumbTextureID = value; }
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
                list.Add(mStartTextureID);
                list.Add(mMiddleTextureID);
                list.Add(mEndTextureID);
                list.Add(mThumbTextureID);

                return list;
            }
        }

        /// <summary>
        /// Gets / Sets the min range value
        /// </summary>
        [Category("Behaviour")]
        public int Min 
        {
            get { return mMin; }
            set
            {
                mMin = Math.Min(value, mMax);
            }
        }

        /// <summary>
        /// Gets / sets the max range value
        /// </summary>
        [Category("Behaviour")]
        public int Max
        {
            get { return mMax; }
            set
            {
                mMax = Math.Max(value, mMin);
            }
        }

        /// <summary>
        /// Gets / sets the slider step
        /// </summary>
        [Category("Behaviour")]
        public int Step
        {
            get { return mStep; }
            set
            {
                mStep = Math.Max(0, value);
            }
        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public GUISlider()
        {
            Layout = new SliderLayout();
            Name = "Slider";

            ThumbHeight = 25;
            ThumbWidth = 25;
        }

        /// <summary>
        /// Applies the provided keyframe to the control,
        /// overriding all needed properties
        /// </summary>
        /// <param name="frame"></param>
        public override void ApplyKeyFrame(KeyFrame startFrame, KeyFrame endFrame, float factor)
        {
            base.ApplyKeyFrame(startFrame, endFrame, factor);

            SliderLayout startLayout = startFrame != null ? startFrame.Layout as SliderLayout : null;
            SliderLayout endLayout = endFrame != null ? endFrame.Layout as SliderLayout : null;

            if (startLayout == null && endLayout == null)
                return;

            int r, g, b, a;

            if (endLayout == null)
            {
                r = startLayout.Color.R;
                g = startLayout.Color.G;
                b = startLayout.Color.B;
                a = startLayout.Color.A;
            }
            else
            {
                r = (int)((float)startLayout.Color.R * (1.0f - factor) + (float)endLayout.Color.R * factor);
                g = (int)((float)startLayout.Color.G * (1.0f - factor) + (float)endLayout.Color.G * factor);
                b = (int)((float)startLayout.Color.B * (1.0f - factor) + (float)endLayout.Color.B * factor);
                a = (int)((float)startLayout.Color.A * (1.0f - factor) + (float)endLayout.Color.A * factor);
            }

            this.Color = Color.FromArgb(a, r, g, b);
        }

        /// <summary>
        /// Draws the Slider
        /// </summary>
        public override void Draw(Otter.Interface.Graphics graphics)
        {
            TextureInfo info = null;

            info = Scene.GetTextureInfo(mStartTextureID);
            int startTexID = (info != null) ? info.TextureID : -1;

            info = Scene.GetTextureInfo(mMiddleTextureID);
            int middleTexID = (info != null) ? info.TextureID : -1;

            info = Scene.GetTextureInfo(mEndTextureID);
            int endTexID = (info != null) ? info.TextureID : -1;

            info = Scene.GetTextureInfo(mThumbTextureID);
            int thumbTexID = (info != null) ? info.TextureID : -1;

            float x = 0.0f;
            float y = Layout.Size.Height / 2.0f - ThumbHeight / 2.0f;
            float midWidth = Math.Max(0.0f, Layout.Size.Width - ThumbWidth * 2.0f);
            int col = ((SliderLayout)Layout).Color.ToArgb();

            graphics.DrawRectangle(startTexID, x, y, ThumbWidth, ThumbHeight, col);

            x += ThumbWidth;
            graphics.DrawRectangle(middleTexID, x, y, midWidth, ThumbHeight, col);

            x += midWidth;
            graphics.DrawRectangle(endTexID, x, y, ThumbWidth, ThumbHeight, col);

            x = ThumbWidth + midWidth / 2.0f - ThumbWidth / 2.0f;
            graphics.DrawRectangle(thumbTexID, x, y, ThumbWidth, ThumbHeight, col);
        }

        /// <summary>
        /// Exports the GUI Slider to disk
        /// </summary>
        /// <param name="bw"></param>
        public override void Export(PlatformBinaryWriter bw)
        {
            FourCCStack fourCCStack = new FourCCStack(bw);
            fourCCStack.Push("GSLD");
            {
                base.Export(bw);

                bw.Write(ThumbWidth);
                bw.Write(ThumbHeight);

                bw.Write(Scene.GetUniqueTextureID(StartTexture));
                bw.Write(Scene.GetUniqueTextureID(MiddleTexture));
                bw.Write(Scene.GetUniqueTextureID(EndTexture));
                bw.Write(Scene.GetUniqueTextureID(ThumbTexture));

                bw.Write(Min);
                bw.Write(Max);
                bw.Write(Step);
                bw.Write((UInt32)0);
                
                bw.Write(Color.White.ToArgb());
            }
            fourCCStack.Pop();
        }

        /// <summary>
        /// Clones (deep copies) the GUI Slider
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            GUISlider ret = new GUISlider();
            if (ret == null) 
                throw new OutOfMemoryException("GUISlider.Clone() failed its allocation.");

            // GUIControl level copy
            ret.ID = this.ID;
            ret.Parent = this.Parent;
            ret.Name = this.Name;
            ret.Layout = this.Layout.Clone() as SliderLayout;
            ret.Scene = this.Scene;
            ret.AnchorFlags = this.AnchorFlags;
            ret.Mask = this.Mask;

            // GUISlider level copy
            ret.ThumbWidth = ThumbWidth;
            ret.ThumbHeight = ThumbHeight;
            ret.StartTexture = this.StartTexture;
            ret.MiddleTexture = this.MiddleTexture;
            ret.EndTexture = this.EndTexture;
            ret.ThumbTexture = this.ThumbTexture;
            ret.Step = this.Step;

            return ret;
        }
    }
}
