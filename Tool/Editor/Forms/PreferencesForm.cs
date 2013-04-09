using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Otter.Editor.Forms
{
    public partial class PreferencesForm : Form
    {
        #region Data
        private Color mBackgroundColor = Color.Empty;
        private Color mSelectColor = Color.Empty;
        private Color mInnerSelectColor = Color.Empty;
        private Color mBoundsColor = Color.Empty;
        private Bitmap mBackgroundImage = null;
        #endregion

        public PreferencesForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Called when the preferences sceneView is loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PreferencesForm_Load(object sender, EventArgs e)
        {
            mShowWelcomeCheckbox.Checked = Otter.Editor.Properties.Settings.Default.ShowWelcome;
            mSaveOnExportCheckbox.Checked = Otter.Editor.Properties.Settings.Default.SaveOnExport;

            mBackgroundColor = Otter.Editor.Properties.Settings.Default.ViewBackgroundColor;
            mSelectColor = Otter.Editor.Properties.Settings.Default.ViewSelectColor;
            mInnerSelectColor = Otter.Editor.Properties.Settings.Default.ViewInnerSelectColor;
            mBoundsColor = Otter.Editor.Properties.Settings.Default.ViewBoundsColor;
            mBackgroundImage = Utils.LoadBitmapResource("Otter.Editor.img.view_background.png");
            mGridIncrement.Value = Otter.Editor.Properties.Settings.Default.ViewGridIncrement;
            mSnapTolerance.Value = Otter.Editor.Properties.Settings.Default.ViewSnapTolerance;
            mAnchorEditor.AnchorFlags = Otter.Editor.Properties.Settings.Default.AnchorFlags;

            if (Otter.Editor.Properties.Settings.Default.ViewBackgroundMode == Otter.Editor.BackgroundMode.Color)
                mColorBackgroundRadioButton.Select();
            else
                mImageBackgroundRadioButton.Select();
        }

        /// <summary>
        /// Called when the Color Panel needs to be painted
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mColorPanel_Paint(object sender, PaintEventArgs e)
        {
            SolidBrush brush = new SolidBrush(mBackgroundColor);
            e.Graphics.FillRectangle(brush, mColorPanel.ClientRectangle);
            brush.Dispose();
        }

        /// <summary>
        /// Called when the Image Panel needs to be painted
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImagePanel_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(mBackgroundImage, 0, 0);
        }

        /// <summary>
        /// User hit the "OK" button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mOKButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;

            Otter.Editor.Properties.Settings.Default.ShowWelcome = mShowWelcomeCheckbox.Checked;
            Otter.Editor.Properties.Settings.Default.SaveOnExport = mSaveOnExportCheckbox.Checked;
            Otter.Editor.Properties.Settings.Default.ViewBackgroundColor = mBackgroundColor;
            Otter.Editor.Properties.Settings.Default.ViewBackgroundMode = (mColorBackgroundRadioButton.Checked ? Otter.Editor.BackgroundMode.Color : Otter.Editor.BackgroundMode.Image);
            Otter.Editor.Properties.Settings.Default.ViewSelectColor = mSelectColor;
            Otter.Editor.Properties.Settings.Default.ViewInnerSelectColor = mInnerSelectColor;
            Otter.Editor.Properties.Settings.Default.ViewBoundsColor = mBoundsColor;
            Otter.Editor.Properties.Settings.Default.ViewGridIncrement = (int)mGridIncrement.Value;
            Otter.Editor.Properties.Settings.Default.ViewSnapTolerance = (int)mSnapTolerance.Value;
            Otter.Editor.Properties.Settings.Default.AnchorFlags = mAnchorEditor.AnchorFlags;

            Otter.Editor.Properties.Settings.Default.Save();

            this.Close();
        }

        /// <summary>
        /// User hit the "Cancel" button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mCancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// User clicked on the image panel.  Select the radio button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mImagePanel_Click(object sender, EventArgs e)
        {
            mImageBackgroundRadioButton.Select();
        }

        /// <summary>
        /// User clicked on the image panel.  Select the radio button and bring up
        /// the color picker dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mColorPanel_Click(object sender, EventArgs e)
        {
            mColorBackgroundRadioButton.Select();

            ColorDialog colorDialog = new ColorDialog();
            colorDialog.Color = mBackgroundColor;
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                mBackgroundColor = colorDialog.Color;
                mColorPanel.Invalidate();
            }
        }

        private void mSelectColorPanel_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.Color = mSelectColor;
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                mSelectColor = colorDialog.Color;
                mSelectColorPanel.Invalidate();
            }
        }

        private void mSelectColorPanel_Paint(object sender, PaintEventArgs e)
        {
            SolidBrush brush = new SolidBrush(mSelectColor);
            e.Graphics.FillRectangle(brush, mColorPanel.ClientRectangle);
            brush.Dispose();
        }

        private void mInnerSelectColorPanel_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.Color = mInnerSelectColor;
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                mInnerSelectColor = colorDialog.Color;
                mInnerSelectColorPanel.Invalidate();
            }
        }

        private void mInnerSelectColorPanel_Paint(object sender, PaintEventArgs e)
        {
            SolidBrush brush = new SolidBrush(mInnerSelectColor);
            e.Graphics.FillRectangle(brush, mColorPanel.ClientRectangle);
            brush.Dispose();
        }

        private void mBoundsColorPanel_Paint(object sender, PaintEventArgs e)
        {
            SolidBrush brush = new SolidBrush(mBoundsColor);
            e.Graphics.FillRectangle(brush, mBoundsColorPanel.ClientRectangle);
            brush.Dispose();
        }

        private void mBoundsColorPanel_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.Color = mBoundsColor;
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                mBoundsColor = colorDialog.Color;
                mBoundsColorPanel.Invalidate();
            }
        }
    }
}
