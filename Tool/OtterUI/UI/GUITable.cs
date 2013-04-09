using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Xml.Serialization;

using Otter;
using Otter.UI;
using Otter.Export;
using Otter.Interface;

namespace Otter.UI
{
    /// <summary>
    /// GUITable's layout
    /// </summary>
    public class TableLayout : ControlLayout
    {
        /// <summary>
        /// Export override
        /// </summary>
        /// <param name="bw"></param>
        public override void Export(PlatformBinaryWriter bw)
        {
            FourCCStack fourCCStack = new FourCCStack(bw);
            fourCCStack.Push("TBLT");
            {
                ExportBase(bw);
            }
            fourCCStack.Pop();
        }

        /// <summary>
        /// Clone Override
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return base.Clone();
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
            TableLayout layout = (TableLayout)obj;
            return (base.Equals(obj));
        }
    }

    /// <summary>
    /// GUITable Control
    /// </summary>
    public class GUITable : GUIControl
    {
        #region Properties
        /// <summary>
        /// Gets / Sets the default row height
        /// </summary>
        [DisplayName("Default Row Height")]
        [Category("Behaviour")]
        public uint DefaultRowHeight { get; set; }

        /// <summary>
        /// Gets / Sets the spacing between rows
        /// </summary>
        [DisplayName("Row Spacing")]
        [Category("Behaviour")]
        public uint RowSpacing { get; set; }
        #endregion

        public GUITable()
        {
            Name = "Table";
            Layout = new TableLayout();

            RowSpacing = 0;
            DefaultRowHeight = 50;
        }

        /// <summary>
        /// Draws the table
        /// </summary>
        /// <param name="graphics"></param>
        public override void Draw(Otter.Interface.Graphics graphics)
        {
            int rowIndex = 0;
            int colEven = Color.FromArgb(200, Color.Gainsboro).ToArgb();
            int colOdd = Color.FromArgb(200, Color.WhiteSmoke).ToArgb();

            for (float y = 0; y < Layout.Size.Height; y += (float)(DefaultRowHeight + RowSpacing))
            {
                float height = (Layout.Size.Height - y) > DefaultRowHeight ? DefaultRowHeight : (Layout.Size.Height - y);
                int col = (rowIndex == 0) ? colEven : colOdd;

                graphics.DrawRectangle(-1, 0.0f, y, Layout.Size.Width, height, col);

                rowIndex = (++rowIndex) % 2;
            }
        }

        /// <summary>
        /// Exports the table
        /// </summary>
        /// <param name="bw"></param>
        public override void Export(Otter.Export.PlatformBinaryWriter bw)
        {
            FourCCStack fourCCStack = new FourCCStack(bw);
            fourCCStack.Push("GTBL");
            {
                base.Export(bw);

                bw.Write(DefaultRowHeight);
                bw.Write(RowSpacing);
            }
            fourCCStack.Pop();
        }

        /// <summary>
        /// Clones the table
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            GUITable ret = new GUITable();

            // GUIControl level copy
            ret.ID = this.ID;
            ret.Name = Name;
            ret.Layout = this.Layout.Clone() as TableLayout;
            ret.Scene = Scene;
            ret.Parent = Parent;
            ret.AnchorFlags = this.AnchorFlags;
            ret.Mask = this.Mask;

            // GUITable level copy
            ret.DefaultRowHeight = this.DefaultRowHeight;
            ret.RowSpacing = this.RowSpacing;

            return ret;
        }
    }
}
