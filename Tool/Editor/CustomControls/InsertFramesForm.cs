using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Otter.Editor.CustomControls
{
    /// <summary>
    /// Form to specify how many frames to insert in the timeline.
    /// </summary>
    public partial class InsertFramesForm : Form
    {
        #region Properties
        /// <summary>
        /// Retrieves the number of frames specified in the control
        /// </summary>
        public int NumFrames
        {
            get
            {
                return (int)mFrameCountNumericControl.Value;
            }
        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public InsertFramesForm()
        {
            InitializeComponent();
            mFrameCountNumericControl.Value = 1;
        }

        /// <summary>
        /// Called when the OK button was clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mOKButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
