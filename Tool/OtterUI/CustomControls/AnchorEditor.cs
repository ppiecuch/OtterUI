using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Otter.UI;

namespace Otter.CustomControls
{
    /// <summary>
    /// UserControl to allow the manipulation of anchor settings
    /// </summary>
    public partial class AnchorEditor : UserControl
    {
        #region Enumerations
        private enum AnchorType
        {
            None,
            Absolute,
            Relative
        }
        #endregion

        #region Data
        private AnchorFlags mAnchorFlags = AnchorFlags.None;
        private const int mMargin = 25;
        #endregion

        #region Properties
        /// <summary>
        /// Gets / Sets the current anchor flags
        /// </summary>
        public AnchorFlags AnchorFlags
        {
            get { return mAnchorFlags; }
            set { mAnchorFlags = value; }
        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public AnchorEditor()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Paints this control
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {            
            Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);

            // Fill the background
            e.Graphics.FillRectangle(Brushes.White, rect);

            // Shrink the rect to determine the area of the control visualization
            rect.Inflate(-(mMargin + 2), -(mMargin + 2));

            // Quit if it shrank too much
            if(rect.Width == 0 || rect.Height == 0)
                return;

            // Draw the inner square representing the control
            e.Graphics.FillRectangle(Brushes.LightBlue, rect);
            e.Graphics.DrawRectangle(Pens.DarkBlue, rect);

            AnchorType type = AnchorType.None;

            // Top
            type = (mAnchorFlags & AnchorFlags.Top) == AnchorFlags.Top ? AnchorType.Absolute : ((mAnchorFlags & AnchorFlags.TopRelative) == AnchorFlags.TopRelative ? AnchorType.Relative : AnchorType.None);
            DrawMarker(e.Graphics, this.Width / 2, mMargin / 2, false, type);

            // Bottom
            type = (mAnchorFlags & AnchorFlags.Bottom) == AnchorFlags.Bottom ? AnchorType.Absolute : ((mAnchorFlags & AnchorFlags.BottomRelative) == AnchorFlags.BottomRelative ? AnchorType.Relative : AnchorType.None);
            DrawMarker(e.Graphics, this.Width / 2, this.Height - mMargin / 2 - 1, false, type);

            // Left
            type = (mAnchorFlags & AnchorFlags.Left) == AnchorFlags.Left ? AnchorType.Absolute : ((mAnchorFlags & AnchorFlags.LeftRelative) == AnchorFlags.LeftRelative ? AnchorType.Relative : AnchorType.None);
            DrawMarker(e.Graphics, mMargin / 2, this.Height / 2, true, type);

            // Right
            type = (mAnchorFlags & AnchorFlags.Right) == AnchorFlags.Right ? AnchorType.Absolute : ((mAnchorFlags & AnchorFlags.RightRelative) == AnchorFlags.RightRelative ? AnchorType.Relative : AnchorType.None);
            DrawMarker(e.Graphics, this.Width - mMargin / 2 - 1, this.Height / 2, true, type);
        }

        /// <summary>
        /// Draws the marker
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="bHorizontal"></param>
        /// <param name="anchorType"></param>
        private void DrawMarker(Graphics g, int x, int y, bool bHorizontal, AnchorType anchorType)
        {
            Pen pen = (anchorType == AnchorType.None) ? Pens.LightGray : Pens.Black;
            if (bHorizontal)
            {
                g.DrawLine(pen, x - mMargin / 2, y, x + mMargin / 2, y);
                g.DrawLine(pen, x - mMargin / 2, y - 2, x - mMargin / 2, y + 2);
                g.DrawLine(pen, x + mMargin / 2, y - 2, x + mMargin / 2, y + 2);
            }
            else
            {
                g.DrawLine(pen, x, y - mMargin / 2, x, y + mMargin / 2);
                g.DrawLine(pen, x - 2, y - mMargin / 2, x + 2, y - mMargin / 2);
                g.DrawLine(pen, x - 2, y + mMargin / 2, x + 2, y + mMargin / 2);
            }

            if (anchorType == AnchorType.Relative)
            {
                string str = "%";
                SizeF dim = g.MeasureString(str, this.Font);

                g.FillRectangle(Brushes.White, x - dim.Width / 2, y - dim.Height / 2, dim.Width, dim.Height);
                g.DrawString(str, this.Font, Brushes.Black, x - dim.Width / 2, y - dim.Height / 2);
            }
        }

        /// <summary>
        /// Called when the mouse button has been pressed down on this control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AnchorEditor_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int hw = this.Width / 2;    // Half Width
                int hh = this.Height / 2;   // Half Height

                // Top or Bottom
                if (e.X >= (hw - 5) && e.X <= (hw + 5))
                {
                    // Top
                    if (e.Y < mMargin)
                    {
                        if ((mAnchorFlags & AnchorFlags.Top) == AnchorFlags.Top)
                        {
                            mAnchorFlags &= ~AnchorFlags.Top;
                            mAnchorFlags |= AnchorFlags.TopRelative;
                        }
                        else if ((mAnchorFlags & AnchorFlags.TopRelative) == AnchorFlags.TopRelative)
                        {
                            mAnchorFlags &= ~AnchorFlags.TopRelative;
                        }
                        else
                        {
                            mAnchorFlags |= AnchorFlags.Top;
                        }
                    }
                    // Bottom
                    else if (e.Y > (this.Height - mMargin))
                    {
                        if ((mAnchorFlags & AnchorFlags.Bottom) == AnchorFlags.Bottom)
                        {
                            mAnchorFlags &= ~AnchorFlags.Bottom;
                            mAnchorFlags |= AnchorFlags.BottomRelative;
                        }
                        else if ((mAnchorFlags & AnchorFlags.BottomRelative) == AnchorFlags.BottomRelative)
                        {
                            mAnchorFlags &= ~AnchorFlags.BottomRelative;
                        }
                        else
                        {
                            mAnchorFlags |= AnchorFlags.Bottom;
                        }
                    }
                }
                // Left or Right
                else if (e.Y >= (hh - 5) && e.Y <= (hh + 5))
                {
                    // Left
                    if (e.X < mMargin)
                    {
                        if ((mAnchorFlags & AnchorFlags.Left) == AnchorFlags.Left)
                        {
                            mAnchorFlags &= ~AnchorFlags.Left;
                            mAnchorFlags |= AnchorFlags.LeftRelative;
                        }
                        else if ((mAnchorFlags & AnchorFlags.LeftRelative) == AnchorFlags.LeftRelative)
                        {
                            mAnchorFlags &= ~AnchorFlags.LeftRelative;
                        }
                        else
                        {
                            mAnchorFlags |= AnchorFlags.Left;
                        }
                    }
                    // Right
                    else if (e.X > (this.Width- mMargin))
                    {
                        if ((mAnchorFlags & AnchorFlags.Right) == AnchorFlags.Right)
                        {
                            mAnchorFlags &= ~AnchorFlags.Right;
                            mAnchorFlags |= AnchorFlags.RightRelative;
                        }
                        else if ((mAnchorFlags & AnchorFlags.RightRelative) == AnchorFlags.RightRelative)
                        {
                            mAnchorFlags &= ~AnchorFlags.RightRelative;
                        }
                        else
                        {
                            mAnchorFlags |= AnchorFlags.Right;
                        }
                    }
                }

                this.Invalidate();
            }
        }
    }
}
