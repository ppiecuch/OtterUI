using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Xml.Serialization;
using System.ComponentModel;

using Otter.Export;
using Otter.Interface;

namespace Otter.UI
{
    /// <summary>
    /// Sprite layout data
    /// </summary>
    public class GroupLayout : ControlLayout
    {
        #region Data
        #endregion

        #region Properties
        #endregion

        /// <summary>
        /// Clones the layout
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            GroupLayout layout = new GroupLayout();
            layout.SetFrom(this);

            return layout;
        }

        /// <summary>
        /// Exports the layout to binary format
        /// </summary>
        /// <param name="bw"></param>
        public override void Export(PlatformBinaryWriter bw)
        {
            FourCCStack fourCCStack = new FourCCStack(bw);
            fourCCStack.Push("GPLT");
            {
                ExportBase(bw);
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
            GroupLayout layout = (GroupLayout)obj;
            return (base.Equals(obj));
        }
    }

    /// <summary>
    /// The group control simply groups a number of controls together.
    /// </summary>
    public class GUIGroup : GUIControl
    {
        #region Data
        private Color mColor = Color.FromArgb(75, Color.Gray);
        #endregion

        #region Properties
        /// <summary>
        /// Returns the list of used texture IDs
        /// </summary>
        public override List<int> TextureIDs
        {
            get
            {
                List<int> list = new List<int>();

                foreach (GUIControl control in Controls)
                {
                    foreach (int textureID in control.TextureIDs)
                    {
                        if(!list.Contains(textureID))
                            list.Add(textureID);
                    }
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
        /// Default Constructor
        /// </summary>
        public GUIGroup()
        {
            Layout = new GroupLayout();
            Name = "Group";
        }

        /// <summary>
        /// Draws the group
        /// </summary>
        public override void Draw(Otter.Interface.Graphics graphics)
        {
            graphics.DrawRectangle(-1, 0.0f, 0.0f, Layout.Size.Width, Layout.Size.Height, mColor.ToArgb());
            base.Draw(graphics);
        }

        /// <summary>
        /// Exports the group
        /// </summary>
        /// <param name="bw"></param>
        public override void Export(PlatformBinaryWriter bw)
        {
            FourCCStack fourCCStack = new FourCCStack(bw);
            fourCCStack.Push("GGRP");
            {
                base.Export(bw);
                base.ExportControls(bw);
            }
            fourCCStack.Pop();
        }

        /// <summary>
        /// Clones (deep copies) the GUI Window
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            GUIGroup ret = new GUIGroup();
            if (ret == null) 
                throw new OutOfMemoryException("GUIGroup.Clone() failed its allocation.");

            // GUIControl level copy
            ret.ID              = this.ID;
            ret.Name            = this.Name;
            ret.Layout          = this.Layout.Clone() as GroupLayout;
            ret.Scene           = this.Scene;
            ret.Parent          = this.Parent;
            ret.AnchorFlags     = this.AnchorFlags;
            ret.Mask            = this.Mask;

            // GUIWindow level copy
            ret.Color           = this.Color;

            foreach (GUIControl control in Controls)
            {
                ret.Controls.Add((GUIControl)control.Clone());
            }

            return ret;
        }
    }
}
