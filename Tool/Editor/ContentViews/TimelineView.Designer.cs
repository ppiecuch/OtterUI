namespace Otter.Editor.ContentViews
{
    partial class TimelineView
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TimelineView));
            this.panel1 = new System.Windows.Forms.Panel();
            this.mTimelineControl = new Otter.Editor.CustomControls.TimelineControl();
            this.mToolstrip = new System.Windows.Forms.ToolStrip();
            this.mManageAnimationsToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.mAnimationsToolStripComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.mFilterToolStripComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mAutoKeyToolstripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mPlayButton = new System.Windows.Forms.ToolStripButton();
            this.mPauseButton = new System.Windows.Forms.ToolStripButton();
            this.mKeyOffButton = new System.Windows.Forms.ToolStripButton();
            this.panel1.SuspendLayout();
            this.mToolstrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.mTimelineControl);
            this.panel1.Controls.Add(this.mToolstrip);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(838, 446);
            this.panel1.TabIndex = 0;
            // 
            // mTimelineControl
            // 
            this.mTimelineControl.Animation = null;
            this.mTimelineControl.BackColor = System.Drawing.SystemColors.Control;
            this.mTimelineControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mTimelineControl.ChannelFilter = Otter.Editor.CustomControls.TimelineControl.Filter.All;
            this.mTimelineControl.CommandManager = null;
            this.mTimelineControl.CurrentFrame = ((uint)(0u));
            this.mTimelineControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mTimelineControl.Location = new System.Drawing.Point(0, 42);
            this.mTimelineControl.Name = "mTimelineControl";
            this.mTimelineControl.Size = new System.Drawing.Size(838, 404);
            this.mTimelineControl.TabIndex = 4;
            this.mTimelineControl.View = null;
            // 
            // mToolstrip
            // 
            this.mToolstrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mManageAnimationsToolStripButton,
            this.mAnimationsToolStripComboBox,
            this.toolStripSeparator3,
            this.toolStripLabel1,
            this.mFilterToolStripComboBox,
            this.toolStripSeparator1,
            this.mAutoKeyToolstripButton,
            this.toolStripSeparator2,
            this.mPlayButton,
            this.mPauseButton,
            this.mKeyOffButton});
            this.mToolstrip.Location = new System.Drawing.Point(0, 0);
            this.mToolstrip.Name = "mToolstrip";
            this.mToolstrip.Size = new System.Drawing.Size(838, 42);
            this.mToolstrip.TabIndex = 0;
            this.mToolstrip.Text = "toolStrip1";
            // 
            // mManageAnimationsToolStripButton
            // 
            this.mManageAnimationsToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.mManageAnimationsToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("mManageAnimationsToolStripButton.Image")));
            this.mManageAnimationsToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.mManageAnimationsToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mManageAnimationsToolStripButton.Name = "mManageAnimationsToolStripButton";
            this.mManageAnimationsToolStripButton.Size = new System.Drawing.Size(36, 39);
            this.mManageAnimationsToolStripButton.Text = "toolStripButton1";
            this.mManageAnimationsToolStripButton.ToolTipText = "Manage Animations";
            this.mManageAnimationsToolStripButton.Click += new System.EventHandler(this.mManageAnimationsToolStripButton_Click);
            // 
            // mAnimationsToolStripComboBox
            // 
            this.mAnimationsToolStripComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mAnimationsToolStripComboBox.Name = "mAnimationsToolStripComboBox";
            this.mAnimationsToolStripComboBox.Size = new System.Drawing.Size(121, 42);
            this.mAnimationsToolStripComboBox.SelectedIndexChanged += new System.EventHandler(this.mAnimationsToolStripComboBox_SelectedIndexChanged);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 42);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(39, 39);
            this.toolStripLabel1.Text = "Show:";
            // 
            // mFilterToolStripComboBox
            // 
            this.mFilterToolStripComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mFilterToolStripComboBox.Items.AddRange(new object[] {
            "All Controls",
            "Animated Controls"});
            this.mFilterToolStripComboBox.Name = "mFilterToolStripComboBox";
            this.mFilterToolStripComboBox.Size = new System.Drawing.Size(121, 42);
            this.mFilterToolStripComboBox.SelectedIndexChanged += new System.EventHandler(this.mFilterToolStripComboBox_SelectedIndexChanged);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 42);
            // 
            // mAutoKeyToolstripButton
            // 
            this.mAutoKeyToolstripButton.BackColor = System.Drawing.Color.Transparent;
            this.mAutoKeyToolstripButton.CheckOnClick = true;
            this.mAutoKeyToolstripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.mAutoKeyToolstripButton.Image = ((System.Drawing.Image)(resources.GetObject("mAutoKeyToolstripButton.Image")));
            this.mAutoKeyToolstripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.mAutoKeyToolstripButton.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.mAutoKeyToolstripButton.Name = "mAutoKeyToolstripButton";
            this.mAutoKeyToolstripButton.Size = new System.Drawing.Size(36, 39);
            this.mAutoKeyToolstripButton.Text = "Play/Pause";
            this.mAutoKeyToolstripButton.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.mAutoKeyToolstripButton.ToolTipText = "Enable / Disable Auto Keyframes";
            this.mAutoKeyToolstripButton.Click += new System.EventHandler(this.mAutoKeyToolstripButton_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 42);
            // 
            // mPlayButton
            // 
            this.mPlayButton.BackColor = System.Drawing.Color.Transparent;
            this.mPlayButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.mPlayButton.Image = ((System.Drawing.Image)(resources.GetObject("mPlayButton.Image")));
            this.mPlayButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.mPlayButton.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.mPlayButton.Name = "mPlayButton";
            this.mPlayButton.Size = new System.Drawing.Size(36, 39);
            this.mPlayButton.Text = "Play/Pause";
            this.mPlayButton.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.mPlayButton.ToolTipText = "Play / Pause";
            this.mPlayButton.Click += new System.EventHandler(this.mPlayButton_Click);
            // 
            // mPauseButton
            // 
            this.mPauseButton.BackColor = System.Drawing.Color.Transparent;
            this.mPauseButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.mPauseButton.Image = ((System.Drawing.Image)(resources.GetObject("mPauseButton.Image")));
            this.mPauseButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.mPauseButton.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.mPauseButton.Name = "mPauseButton";
            this.mPauseButton.Size = new System.Drawing.Size(36, 39);
            this.mPauseButton.Text = "Pause";
            this.mPauseButton.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.mPauseButton.ToolTipText = "Pause";
            this.mPauseButton.Click += new System.EventHandler(this.mPauseButton_Click);
            // 
            // mKeyOffButton
            // 
            this.mKeyOffButton.BackColor = System.Drawing.Color.Transparent;
            this.mKeyOffButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.mKeyOffButton.Image = ((System.Drawing.Image)(resources.GetObject("mKeyOffButton.Image")));
            this.mKeyOffButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.mKeyOffButton.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.mKeyOffButton.Name = "mKeyOffButton";
            this.mKeyOffButton.Size = new System.Drawing.Size(39, 39);
            this.mKeyOffButton.Text = "Key Off";
            this.mKeyOffButton.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.mKeyOffButton.ToolTipText = "Key Off";
            this.mKeyOffButton.Click += new System.EventHandler(this.mKeyOffButton_Click);
            // 
            // TimelineView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(838, 446);
            this.Controls.Add(this.panel1);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "TimelineView";
            this.TabText = "Timeline";
            this.Text = "Timeline";
            this.Load += new System.EventHandler(this.TimelineView_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.mToolstrip.ResumeLayout(false);
            this.mToolstrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStrip mToolstrip;
        private System.Windows.Forms.ToolStripComboBox mAnimationsToolStripComboBox;
        private Otter.Editor.CustomControls.TimelineControl mTimelineControl;
        private System.Windows.Forms.ToolStripButton mManageAnimationsToolStripButton;
        private System.Windows.Forms.ToolStripButton mPauseButton;
        private System.Windows.Forms.ToolStripButton mKeyOffButton;
        private System.Windows.Forms.ToolStripComboBox mFilterToolStripComboBox;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton mPlayButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton mAutoKeyToolstripButton;
    }
}