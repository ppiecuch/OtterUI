namespace Otter.Editor.Forms
{
    partial class LoadingForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mProgressBar = new System.Windows.Forms.ProgressBar();
            this.mStatusLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // mProgressBar
            // 
            this.mProgressBar.Location = new System.Drawing.Point(12, 12);
            this.mProgressBar.Name = "mProgressBar";
            this.mProgressBar.Size = new System.Drawing.Size(470, 23);
            this.mProgressBar.TabIndex = 0;
            // 
            // mStatusLabel
            // 
            this.mStatusLabel.Location = new System.Drawing.Point(12, 38);
            this.mStatusLabel.Name = "mStatusLabel";
            this.mStatusLabel.Size = new System.Drawing.Size(470, 21);
            this.mStatusLabel.TabIndex = 1;
            this.mStatusLabel.Text = "Status";
            this.mStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LoadingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(494, 64);
            this.ControlBox = false;
            this.Controls.Add(this.mStatusLabel);
            this.Controls.Add(this.mProgressBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "LoadingForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Please wait while loading...";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar mProgressBar;
        private System.Windows.Forms.Label mStatusLabel;
    }
}