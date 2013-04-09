using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Serialization;

using Otter.Export;
using Otter.UI.Animation;
using Otter.Interface;
using Otter.TypeConverters;
using Otter.TypeEditors;
using Otter.Project;
using Otter.UI.Resources;

namespace Otter.UI
{
    /// <summary>
    /// GUIToggle's layout
    /// </summary>
    public class ToggleLayout : ControlLayout
    {
        #region Data
        private Color mColor = Color.White;
        #endregion

        #region Properties
        /// <summary>
        /// Gets / Sets the Toggle's color
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
            ToggleLayout layout = new ToggleLayout();
            layout.SetFrom(this);

            layout.Color = this.Color;

            return layout;
        }

        /// <summary>
        /// Exports the Toggle layout
        /// </summary>
        /// <param name="bw"></param>
        public override void Export(PlatformBinaryWriter bw)
        {
            FourCCStack fourCCStack = new FourCCStack(bw);
            fourCCStack.Push("TGLT");
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
            ToggleLayout layout = (ToggleLayout)obj;
            return (base.Equals(obj) &&
                    layout.Color.Equals(this.Color));
        }
    }

    /// <summary>
    /// GUIToggle Control
    /// </summary>
    public class GUIToggle : GUIControl
    {
        #region Enums
        public enum ToggleState
        {
            On,
            Off
        }
        #endregion

        #region Data
        private int mOnTextureID = -1;
        private int mOffTextureID = -1;
        private ToggleState mDefaultState = ToggleState.On;
        #endregion

        #region Properties
        /// <summary>
        /// Gets / Sets the texture displayed when the toggle is "on"
        /// </summary>
        [TypeConverter(typeof(TextureConverter))]
        [Editor(typeof(UITextureEditor), typeof(UITypeEditor))]
        [DisplayName("On Texture")]
        [Category("Behaviour")]
        public int OnTexture
        {
            get { return mOnTextureID; }
            set { mOnTextureID = value; }
        }

        /// <summary>
        /// Gets / Sets the texture displayed when the toggle is "off"
        /// </summary>
        [TypeConverter(typeof(TextureConverter))]
        [Editor(typeof(UITextureEditor), typeof(UITypeEditor))]
        [DisplayName("Off Texture")]
        [Category("Behaviour")]
        public int OffTexture
        {
            get { return mOffTextureID; }
            set { mOffTextureID = value; }
        }

        /// <summary>
        /// Gets / Sets the default state of the toggle
        /// </summary>
        [DisplayName("Default ToggleState")]
        [Category("Behaviour")]
        public ToggleState DefaultState
        {
            get { return mDefaultState; }
            set { mDefaultState = value; }
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
                list.Add(mOnTextureID);
                list.Add(mOffTextureID);

                return list;
            }
        }

        /// <summary>
        /// Gets / Sets the Toggle's color
        /// </summary>
        [XmlIgnore]
        [Category("Layout")]
        public Color Color
        {
            get { return (Layout as ToggleLayout).Color; }
            set { (Layout as ToggleLayout).Color = value; }
        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public GUIToggle()
        {
            Name = "Toggle";
            Layout = new ToggleLayout();
        }

        /// <summary>
        /// Applies the provided keyframe to the control,
        /// overriding all needed properties
        /// </summary>
        /// <param name="frame"></param>
        public override void ApplyKeyFrame(KeyFrame startFrame, KeyFrame endFrame, float factor)
        {
            base.ApplyKeyFrame(startFrame, endFrame, factor);

            ToggleLayout startLayout = startFrame != null ? startFrame.Layout as ToggleLayout : null;
            ToggleLayout endLayout = endFrame != null ? endFrame.Layout as ToggleLayout : null;

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
        /// Draws the Toggle
        /// </summary>
        /// <param name="graphics"></param>
        public override void Draw(Otter.Interface.Graphics graphics)
        {
            TextureInfo info = Scene.GetTextureInfo(DefaultState == ToggleState.On ? OnTexture : OffTexture);
            int texID = (info != null) ? info.TextureID : -1;
            
            graphics.DrawRectangle(texID, 0.0f, 0.0f, Layout.Size.Width, Layout.Size.Height, ((ToggleLayout)Layout).Color.ToArgb());
        }

        /// <summary>
        /// Exports the Toggle
        /// </summary>
        /// <param name="bw"></param>
        public override void Export(Otter.Export.PlatformBinaryWriter bw)
        {
            FourCCStack fourCCStack = new FourCCStack(bw);
            fourCCStack.Push("GTGL");
            {
                base.Export(bw);

                bw.Write(Scene.GetUniqueTextureID(OnTexture));
                bw.Write(Scene.GetUniqueTextureID(OffTexture));

                bw.Write(Color.White.ToArgb());
            }
            fourCCStack.Pop();
        }

        /// <summary>
        /// Clones the Toggle
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            GUIToggle ret = new GUIToggle();

            // GUIControl level copy
            ret.ID = this.ID;
            ret.Name = this.Name;
            ret.Layout = this.Layout.Clone() as ToggleLayout;
            ret.Scene = this.Scene;
            ret.Parent = this.Parent;
            ret.AnchorFlags = this.AnchorFlags;
            ret.Mask = this.Mask;

            // GUITable level copy
            ret.DefaultState = this.DefaultState;
            ret.OffTexture = this.OffTexture;
            ret.OnTexture = this.OnTexture;

            return ret;
        }
    }
}
