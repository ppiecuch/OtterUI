using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Xml.Serialization;
using System.Linq;

using Otter.Export;
using Otter.UI.Animation;
using Otter.Interface;
using Otter.Project;

namespace Otter.UI
{
    /// <summary>
    /// Sprite layout data
    /// </summary>
    public class LabelLayout : ControlLayout
    {
        #region Data
        private Color mColor = Color.White;
        private SizeF mScale = new SizeF(1.0f, 1.0f);
        private float mSkew = 0.0f;
        private int mDropShadow = 0;
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
        /// Exports the color to xml
        /// </summary>
        [Browsable(false)]
        [XmlElement("Color")]
        public string ColorXML
        {
            get { return TypeDescriptor.GetConverter(typeof(Color)).ConvertToString(mColor); }
            set { mColor = (Color)TypeDescriptor.GetConverter(typeof(Color)).ConvertFrom(value); }
        }

        /// <summary>
        /// Gets / Sets the label's font scale
        /// </summary>
        public SizeF Scale
        {
            get { return mScale; }
            set { mScale = value; }
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
        /// Gets / Sets the drop shadow distance
        /// </summary>
        public int DropShadow
        {
            get { return mDropShadow; }
            set { mDropShadow = value; }
        }
        #endregion

        /// <summary>
        /// Clones the layout
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            LabelLayout layout = new LabelLayout();
            layout.SetFrom(this);

            layout.Color = this.Color;
            layout.Scale = this.Scale;
            layout.Skew = this.Skew;
            layout.DropShadow = this.DropShadow;

            return layout;
        }

        /// <summary>
        /// Exports the label layout
        /// </summary>
        /// <param name="bw"></param>
        public override void Export(PlatformBinaryWriter bw)
        {
            FourCCStack fourCCStack = new FourCCStack(bw);
            fourCCStack.Push("LBLT");
            {
                ExportBase(bw);

                bw.Write(Color);
                bw.Write(Scale.Width);
                bw.Write(Scale.Height);
                bw.Write(Skew);
                bw.Write(DropShadow);
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
            LabelLayout layout = (LabelLayout)obj;
            return (base.Equals(obj) &&
                    layout.Color.Equals(this.Color) &&
                    layout.Scale.Equals(this.Scale));
        }
    }

    /// <summary>
    /// Simply renders text to the screen.
    /// </summary>
    public class GUILabel : GUIControl
    {
        public enum TextFitMode
        {
            Wrap,
            NoWrap,
            ScaleToFit,
            ScaleDown,
            Truncate
        }

        #region Data
        private int mFontID = -1;
        private GUIFont mFont = null;
        private string mText = "";
        private float mLeading = 0.15f;
        private int mTracking = 0;
        private TextFitMode mTextFit = TextFitMode.Wrap;

        private HoriAlignment mHorizontalAlignment = HoriAlignment.Left;
        private VertAlignment mVerticalAlignment = VertAlignment.Top;
        #endregion

        #region Properties
        /// <summary>
        /// Gets / Sets the font ID for the label.
        /// </summary>
        [Category("Text Layout")]
        [TypeConverter(typeof(Otter.TypeConverters.FontConverter))]
        [Editor(typeof(Otter.TypeEditors.UIFontEditor), typeof(UITypeEditor))]
        [DisplayName("Font")]
        public int FontID
        {
            get { return mFontID; }
            set 
            { 
                mFontID = value;
                mFont = GUIProject.CurrentProject.GetFont(mFontID);
            }
        }

        /// <summary>
        /// Gets / Sets the label text
        /// </summary>
        [Category("Text Layout")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string Text
        {
            get { return mText; }
            set 
            {
                if (mText != value)
                {
                    mText = value;
                }
            }
        }

        /// <summary>
        /// Gets / Sets the text fit mode
        /// </summary>
        [Category("Text Layout")]
        [DisplayName("Text Fit")]
        public Otter.UI.GUILabel.TextFitMode TextFit
        {
            get { return mTextFit; }
            set { mTextFit = value; }
        }

        /// <summary>
        /// Gets / Sets the sprite's color
        /// </summary>
        [XmlIgnore]
        [Category("Layout")]
        public Color Color
        {
            get { return (Layout as LabelLayout).Color; }
            set { (Layout as LabelLayout).Color = value; }
        }

        /// <summary>
        /// Gets / Sets the label's font scale
        /// </summary>
        [XmlIgnore]
        [Category("Layout")]
        public SizeF Scale
        {
            get { return (Layout as LabelLayout).Scale; }
            set { (Layout as LabelLayout).Scale = value; }
        }

        /// <summary>
        /// Gets / Sets the skew angle, in degrees
        /// </summary>
        [XmlIgnore]
        [Category("Layout")]
        public float Skew
        {
            get { return (Layout as LabelLayout).Skew; }
            set { (Layout as LabelLayout).Skew = value; }
        }

        /// <summary>
        /// Gets / Sets the drop shadow distance
        /// </summary>
        [XmlIgnore]
        [Category("Layout")]
        [DisplayName("Drop Shadow")]
        public int DropShadow
        {
            get { return (Layout as LabelLayout).DropShadow; }
            set { (Layout as LabelLayout).DropShadow = value; }
        }

        /// <summary>
        /// Gets / Sets the horizontal text alignment
        /// </summary>
        [Category("Alignment")]
        [XmlAttribute("HAlign")]
        [DisplayName("Horizontal")]
        public HoriAlignment HorizontalAlignment
        {
            get { return mHorizontalAlignment; }
            set { mHorizontalAlignment = value; }
        }

        /// <summary>
        /// Gets / Sets the vertical text alignment
        /// </summary>
        [Category("Alignment")]
        [XmlAttribute("VAlign")]
        [DisplayName("Vertical")]
        public VertAlignment VerticalAlignment
        {
            get { return mVerticalAlignment; }
            set { mVerticalAlignment = value; }
        }

        /// <summary>
        /// Gets / Sets the font leading
        /// </summary>
        [Category("Text Layout")]
        [TypeConverter(typeof(Otter.TypeConverters.PercentageConverter))]
        public float Leading
        {
            get { return mLeading; }
            set { mLeading = value; }
        }

        /// <summary>
        /// Gets / Sets the font tracking
        /// </summary>
        [Category("Text Layout")]
        public int Tracking
        {
            get { return mTracking; }
            set { mTracking = value; }
        }
        #endregion
         
        /// <summary>
        /// Constructor
        /// </summary>
        public GUILabel()
        {
            Layout = new LabelLayout();
            FontID = -1;
            Text = "Label Text";
            Name = "Label";
        }

        /// <summary>
        /// Constructs the label with an initial string and font ID
        /// </summary>
        /// <param name="text"></param>
        /// <param name="fontID"></param>
        public GUILabel(string text, int fontID)
        {
            Layout = new LabelLayout();
            FontID = fontID;
            Text = text;
            Name = "Label";
        }

        /// <summary>
        /// Applies the provided keyframe to the control,
        /// overriding all needed properties
        /// </summary>
        /// <param name="frame"></param>
        public override void ApplyKeyFrame(KeyFrame startFrame, KeyFrame endFrame, float factor)
        {
            base.ApplyKeyFrame(startFrame, endFrame, factor);

            if (startFrame == null && endFrame == null)
                return;

            LabelLayout startLayout = startFrame != null ? startFrame.Layout as LabelLayout : null;
            LabelLayout endLayout   = endFrame != null ? endFrame.Layout as LabelLayout : null;

            int r, g, b, a;
            float scaleX, scaleY;
            float skew;
            int dropShadow;

            if (endLayout == null)
            {
                r = startLayout.Color.R;
                g = startLayout.Color.G;
                b = startLayout.Color.B;
                a = startLayout.Color.A;

                scaleX = startLayout.Scale.Width;
                scaleY = startLayout.Scale.Height;

                skew = startLayout.Skew;

                dropShadow = startLayout.DropShadow;
            }
            else
            {
                r = (int)((float)startLayout.Color.R * (1.0f - factor) + (float)endLayout.Color.R * factor);
                g = (int)((float)startLayout.Color.G * (1.0f - factor) + (float)endLayout.Color.G * factor);
                b = (int)((float)startLayout.Color.B * (1.0f - factor) + (float)endLayout.Color.B * factor);
                a = (int)((float)startLayout.Color.A * (1.0f - factor) + (float)endLayout.Color.A * factor);

                scaleX = startLayout.Scale.Width * (1.0f - factor) + (float)endLayout.Scale.Width * factor;
                scaleY = startLayout.Scale.Height * (1.0f - factor) + (float)endLayout.Scale.Height * factor;

                skew = startLayout.Skew * (1.0f - factor) + endLayout.Skew * factor;

                dropShadow = (int)(startLayout.DropShadow * (1.0f - factor) + endLayout.DropShadow * factor);
            }

            this.Color = Color.FromArgb(a, r, g, b);
            this.Scale = new SizeF(scaleX, scaleY);
            this.Skew = skew;
            this.DropShadow = dropShadow;
        }

        /// <summary>
        /// Draws the label
        /// </summary>
        public override void Draw(Otter.Interface.Graphics graphics)
        {
            if(mText == "" || mText == null)
                return;

            if (mFont == null)
                return;

            LabelLayout layout = Layout as LabelLayout;
            mFont.Draw(mText, 0, 0, layout.Size.Width, layout.Size.Height, layout.Color, layout.Scale, HorizontalAlignment, VerticalAlignment, mLeading, mTracking, Skew, mTextFit, DropShadow, InheritedMask);
        }

        /// <summary>
        /// Exports the GUI Label to disk
        /// </summary>
        /// <param name="bw"></param>
        public override void Export(PlatformBinaryWriter bw)
        {
            FourCCStack fourCCStack = new FourCCStack(bw);
            fourCCStack.Push("GLBL");

            base.Export(bw);

            bw.Write(mFontID);
            bw.Write(Color.White.ToArgb());
            bw.Write(1.0f); // Scale X
            bw.Write(1.0f); // Scale Y
            bw.Write(0.0f); // Skew
            bw.Write(0); // Drop shadow
            bw.Write((UInt32)mHorizontalAlignment);
            bw.Write((UInt32)mVerticalAlignment);
            bw.Write(mLeading);
            bw.Write(mTracking);
            bw.Write((UInt32)mTextFit);

            byte[] bytes = Utils.StringToBytes(mText, 4);
            bw.Write(bytes.Length);
            bw.Write(bytes);

            fourCCStack.Pop();
        }

        /// <summary>
        /// Clones (deep copies) the GUI Label
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            GUILabel ret = new GUILabel();
            if (ret == null) 
                throw new OutOfMemoryException("GUILabel.Clone() failed its allocation.");

            // GUIControl level copy
            ret.ID = this.ID;
            ret.Parent = this.Parent;
            ret.Name = this.Name;
            ret.Layout = this.Layout.Clone() as LabelLayout;
            ret.Scene = this.Scene;
            ret.AnchorFlags = this.AnchorFlags;
            ret.Mask = this.Mask;

            // GUILabel level copy
            ret.FontID = this.FontID; // sets mFont
            ret.HorizontalAlignment = this.HorizontalAlignment;
            ret.VerticalAlignment = this.VerticalAlignment;
            ret.Text = (string)this.Text.Clone();
            ret.Skew = this.Skew;
            ret.DropShadow = this.DropShadow;
            ret.TextFit = this.TextFit;
            ret.Tracking = this.Tracking;
            ret.Leading = this.Leading;

            return ret;
        }

    }
}
