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
using Otter.TypeConverters;
using Otter.TypeEditors;
using Otter.Project;
using Otter.UI.Resources;

namespace Otter.UI
{
    /// <summary>
    /// Sprite layout data
    /// </summary>
    public class ButtonLayout : ControlLayout
    {
        #region Data
        private Color mTextColor = Color.White;
        private SizeF mTextScale = new SizeF(1.0f, 1.0f);
        #endregion

        #region Properties
        /// <summary>
        /// Gets / Sets the sprite's color
        /// </summary>
        [XmlIgnore]
        [DisplayName("Text Color")]
        public Color TextColor
        {
            get { return mTextColor; }
            set { mTextColor = value; }
        }

        /// <summary>
        /// Exports the color to xml
        /// </summary>
        [Browsable(false)]
        [XmlElement("Color")]
        public string ColorXML
        {
            get { return TypeDescriptor.GetConverter(typeof(Color)).ConvertToString(mTextColor); }
            set { mTextColor = (Color)TypeDescriptor.GetConverter(typeof(Color)).ConvertFrom(value); }
        }

        /// <summary>
        /// Gets / Sets the label's font scale
        /// </summary>
        [DisplayName("Text Scale")]
        public SizeF TextScale
        {
            get { return mTextScale; }
            set { mTextScale = value; }
        }
        #endregion

        /// <summary>
        /// Exports the layout to binary format
        /// </summary>
        /// <param name="bw"></param>
        public override void Export(PlatformBinaryWriter bw)
        {
            FourCCStack fourCCStack = new FourCCStack(bw);
            fourCCStack.Push("BTLT");
            {
                ExportBase(bw);
            }
            fourCCStack.Pop();
        }

        /// <summary>
        /// Clone override
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            ButtonLayout clone = new ButtonLayout();

            clone.SetFrom(this);

            clone.TextColor = this.TextColor;
            clone.TextScale = this.TextScale;

            return clone;
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
            ButtonLayout layout = (ButtonLayout)obj;
            return (base.Equals(obj) && 
                    layout.TextColor.Equals(this.TextColor) &&
                    layout.TextScale == this.TextScale);
        }
    }

    /* A button control is one that can fire events when it's
     * been clicked.
     */
    public class GUIButton : GUIControl
    {
        #region Enums
        public enum ButtonState
        {
            Default,
            Down
        }
        #endregion

        #region Data
        private int mDefaultTextureID = -1;
        private int mDownTextureID = -1;

        private Color mDefaultColor = Color.White;
        private Color mDownColor = Color.White;

        private ButtonState mCurrentState = ButtonState.Default;

        private GUILabel mLabel = new GUILabel();

        private List<Otter.UI.Actions.Action> mOnClickActionList = new List<Otter.UI.Actions.Action>();
        #endregion

        #region Properties
        /// <summary>
        /// Gets / Sets the parent scene for this button
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public override GUIScene Scene
        {
            get
            {
                return base.Scene;
            }
            set
            {
                base.Scene = value;

                // Fix up the actions to point to the parent scene
                foreach (Otter.UI.Actions.Action action in mOnClickActionList)
                {
                    action.Scene = this.Scene;
                }
            }
        }

        /// <summary>
        /// Gets / Sets the sprite's color
        /// </summary>
        [XmlIgnore]
        [DisplayName("Text Color")]
        [Category("Layout")]
        public Color TextColor
        {
            get { return (Layout as ButtonLayout).TextColor; }
            set { (Layout as ButtonLayout).TextColor = value; }
        }

        /// <summary>
        /// Gets / Sets the label's font scale
        /// </summary>
        [XmlIgnore]
        [DisplayName("Text Scale")]
        [Category("Layout")]
        public SizeF TextScale
        {
            get { return (Layout as ButtonLayout).TextScale; }
            set { (Layout as ButtonLayout).TextScale = value; }
        }

        /// <summary>
        /// Gets / Sets the default texture 
        /// </summary>
        [TypeConverter(typeof(TextureConverter))]
        [Editor(typeof(UITextureEditor), typeof(UITypeEditor))]
        [DisplayName("Default Texture")]
        [Category("Appearance")]
        public int DefaultTexture
        {
            get { return mDefaultTextureID; }
            set { mDefaultTextureID = value; }
        }

        /// <summary>
        /// Gets / Sets the down-state texture
        /// </summary>
        [TypeConverter(typeof(TextureConverter))]
        [Editor(typeof(UITextureEditor), typeof(UITypeEditor))]
        [DisplayName("Down Texture")]
        [Category("Appearance")]
        public int DownTexture
        {
            get { return mDownTextureID; }
            set { mDownTextureID = value; }
        }

        /// <summary>
        /// Gets / Sets the default color
        /// </summary>
        [XmlIgnore]
        [DisplayName("Default Color")]
        [Category("Appearance")]
        public Color DefaultColor
        {
            get { return mDefaultColor; }
            set { mDefaultColor = value; }
        }

        /// <summary>
        /// Gets / Sets the down color
        /// </summary>
        [XmlIgnore]
        [DisplayName("Down Color")]
        [Category("Appearance")]
        public Color DownColor
        {
            get { return mDownColor; }
            set { mDownColor = value; }
        }

        /// <summary>
        /// Exports the color to xml
        /// </summary>
        [Browsable(false)]
        [XmlElement("Colors")]
        public string[] Colors
        {
            get 
            {
                string color1 = TypeDescriptor.GetConverter(typeof(Color)).ConvertToString(mDefaultColor);
                string color2 = TypeDescriptor.GetConverter(typeof(Color)).ConvertToString(mDownColor);

                return new string[] { color1, color2 };
            }
            set 
            {
                if (value != null && value.Length == 2)
                {
                    mDefaultColor = (Color)TypeDescriptor.GetConverter(typeof(Color)).ConvertFrom(value[0]);
                    mDownColor = (Color)TypeDescriptor.GetConverter(typeof(Color)).ConvertFrom(value[1]);
                }
            }
        }

        /// <summary>
        /// Gets / Sets the button's current state 
        /// </summary>
        [DisplayName("Default State")]
        [Category("Appearance")]
        public ButtonState CurrentState
        {
            get { return mCurrentState; }
            set { mCurrentState = value; }
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
                list.Add(mDefaultTextureID);
                list.Add(mDownTextureID);

                return list;
            }
        }

        /// <summary>
        /// Returns the list of used sound IDs
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public override List<int> SoundIDs
        {
            get
            {
                List<int> list = new List<int>();

                foreach (Otter.UI.Actions.Action action in mOnClickActionList)
                {
                    if (action is Otter.UI.Actions.SoundAction)
                    {
                        list.Add(((Otter.UI.Actions.SoundAction)action).Sound);
                    }
                }

                return list;
            }
        }

        /// <summary>
        /// Gets / Sets the font ID for the label.
        /// </summary>
        [TypeConverter(typeof(Otter.TypeConverters.FontConverter))]
        [Editor(typeof(Otter.TypeEditors.UIFontEditor), typeof(UITypeEditor))]
        [Category("Appearance")]
        [DisplayName("Font")]
        public int FontID
        {
            get { return mLabel.FontID; }
            set { mLabel.FontID = value; }
        }

        /// <summary>
        /// Gets / Sets the label text
        /// </summary>
        [Category("Appearance")]
        public string Text
        {
            get { return mLabel.Text; }
            set { mLabel.Text = value; }
        }

        /// <summary>
        /// Gets / Sets the horizontal text alignment
        /// </summary>
        [XmlAttribute("HAlign")]
        [Category("Appearance")]
        [DisplayName("Horizontal Alignment")]
        public HoriAlignment HorizontalAlignment
        {
            get { return mLabel.HorizontalAlignment; }
            set { mLabel.HorizontalAlignment = value; }
        }

        /// <summary>
        /// Gets / Sets the vertical text alignment
        /// </summary>
        [XmlAttribute("VAlign")]
        [Category("Appearance")]
        [DisplayName("Vertical Alignment")]
        public VertAlignment VerticalAlignment
        {
            get { return mLabel.VerticalAlignment; }
            set { mLabel.VerticalAlignment = value; }
        }

        /// <summary>
        /// Gets / Sets the action list when the button is clicked
        /// </summary>
        [Category("Behaviour")]
        [DisplayName("On-Click Actions")]
        [Editor(typeof(Otter.TypeEditors.UIActionListEditor), typeof(UITypeEditor))]
        public List<Otter.UI.Actions.Action> OnClickActionList
        {
            get { return mOnClickActionList; }
            set { mOnClickActionList = value; }
        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public GUIButton()
        {
            Name = "Button";
            Layout = new ButtonLayout();
        }

        /// <summary>
        /// Draws the button
        /// </summary>
        public override void Draw(Otter.Interface.Graphics graphics)
        {
            TextureInfo info = Scene.GetTextureInfo(mCurrentState == ButtonState.Default ? mDefaultTextureID : mDownTextureID);
            Color color = mCurrentState == ButtonState.Default ? mDefaultColor : mDownColor;
            int texID = (info != null) ? info.TextureID : -1;

            graphics.DrawRectangle(texID, 0.0f, 0.0f, this.Size.Width, this.Size.Height, color.ToArgb());

            if (mLabel.Text != "")
            {
                GUIFont font = GUIProject.CurrentProject.GetFont(mLabel.FontID);
                if (font != null)
                {
                    ButtonLayout layout = Layout as ButtonLayout;
                    font.Draw(mLabel.Text, 0, 0, layout.Size.Width, layout.Size.Height, layout.TextColor, layout.TextScale, HorizontalAlignment, VerticalAlignment);
                }
            }
        }

        /// <summary>
        /// Exports the GUI Sprite to disk
        /// </summary>
        /// <param name="bw"></param>
        public override void Export(PlatformBinaryWriter bw)
        {
            FourCCStack fourCCStack = new FourCCStack(bw);
            fourCCStack.Push("GBTT");

            base.Export(bw);

            bw.Write(Scene.GetUniqueTextureID(mDefaultTextureID));
            bw.Write(Scene.GetUniqueTextureID(mDownTextureID));
            
            bw.Write(mLabel.FontID);
            bw.Write(DefaultColor.ToArgb());
            bw.Write(DownColor.ToArgb());
            bw.Write(1.0f);
            bw.Write(1.0f);
            bw.Write((UInt32)HorizontalAlignment);
            bw.Write((UInt32)VerticalAlignment);

            bw.Write(OnClickActionList.Count);

            byte[] bytes = Utils.StringToBytes(mLabel.Text, 4);
            bw.Write(bytes.Length);
            bw.Write(bytes);

            foreach (UI.Actions.Action action in this.OnClickActionList)
            {
                action.Export(bw);
            }

            fourCCStack.Pop();
        }

        /// <summary>
        /// Clones (deep copies) this GUI Button
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            GUIButton ret = new GUIButton();
            if (ret == null) 
                throw new OutOfMemoryException("GUIButton.Clone() failed its allocation.");

            // GUIControl level copy
            ret.ID              = this.ID;
            ret.Name            = Name;
            ret.Layout          = this.Layout.Clone() as ButtonLayout;
            ret.Scene           = Scene;
            ret.Parent          = Parent;
            ret.AnchorFlags     = this.AnchorFlags;

            // GUIButton level copy
            ret.DefaultTexture  = DefaultTexture;
            ret.DownColor       = DownColor;
            ret.DefaultColor    = DefaultColor;
            ret.DownTexture     = DownTexture;
            ret.CurrentState    = CurrentState;

            ret.FontID               = this.FontID;
            ret.Text                 = this.Text; 
            ret.TextColor            = this.TextColor;
            ret.TextScale            = this.TextScale;
            ret.VerticalAlignment    = this.VerticalAlignment;
            ret.HorizontalAlignment  = this.HorizontalAlignment;

            foreach (Otter.UI.Actions.Action action in mOnClickActionList)
            {
                ret.OnClickActionList.Add((Otter.UI.Actions.Action)action.Clone());
            }

            return ret;
        }
    }
}
