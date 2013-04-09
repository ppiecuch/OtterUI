namespace Otter.Editor.ContentViews
{
    partial class SceneView
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
            this.mContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mCreateKeyFrameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.mBringToFrontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mSendToBackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.DuplicateStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mDeleteStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.mResolutionComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.mZoomTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.mSafeDisplayComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.mBoundsToolStripDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.hideScreenBoundsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showScreenBoundsRedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showScreenBoundsTransparentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mShowGridToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.mRenderPanel = new Otter.Editor.CustomControls.RenderPanel();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.mCustomActionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mContextMenuStrip.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mContextMenuStrip
            // 
            this.mContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mCreateKeyFrameToolStripMenuItem,
            this.toolStripMenuItem1,
            this.mBringToFrontToolStripMenuItem,
            this.mSendToBackToolStripMenuItem,
            this.toolStripMenuItem2,
            this.DuplicateStripMenuItem,
            this.mDeleteStripMenuItem,
            this.toolStripMenuItem3,
            this.mCustomActionsToolStripMenuItem});
            this.mContextMenuStrip.Name = "mContextMenuStrip";
            this.mContextMenuStrip.Size = new System.Drawing.Size(162, 176);
            this.mContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.mContextMenuStrip_Opening);
            // 
            // mCreateKeyFrameToolStripMenuItem
            // 
            this.mCreateKeyFrameToolStripMenuItem.Name = "mCreateKeyFrameToolStripMenuItem";
            this.mCreateKeyFrameToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.mCreateKeyFrameToolStripMenuItem.Text = "Create Keyframe";
            this.mCreateKeyFrameToolStripMenuItem.Click += new System.EventHandler(this.mCreateKeyFrameToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(158, 6);
            // 
            // mBringToFrontToolStripMenuItem
            // 
            this.mBringToFrontToolStripMenuItem.Name = "mBringToFrontToolStripMenuItem";
            this.mBringToFrontToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.mBringToFrontToolStripMenuItem.Text = "Bring to Front";
            this.mBringToFrontToolStripMenuItem.Click += new System.EventHandler(this.mBringToFrontToolStripMenuItem_Click);
            // 
            // mSendToBackToolStripMenuItem
            // 
            this.mSendToBackToolStripMenuItem.Name = "mSendToBackToolStripMenuItem";
            this.mSendToBackToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.mSendToBackToolStripMenuItem.Text = "Send to Back";
            this.mSendToBackToolStripMenuItem.Click += new System.EventHandler(this.mSendToBackToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(158, 6);
            // 
            // DuplicateStripMenuItem
            // 
            this.DuplicateStripMenuItem.Name = "DuplicateStripMenuItem";
            this.DuplicateStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.DuplicateStripMenuItem.Text = "Duplicate";
            this.DuplicateStripMenuItem.Click += new System.EventHandler(this.mDuplicateStripMenuItem_Click);
            // 
            // mDeleteStripMenuItem
            // 
            this.mDeleteStripMenuItem.Name = "mDeleteStripMenuItem";
            this.mDeleteStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.mDeleteStripMenuItem.Text = "Delete";
            this.mDeleteStripMenuItem.Click += new System.EventHandler(this.mDeleteStripMenuItem_Click);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(66, 22);
            this.toolStripLabel2.Text = "Resolution:";
            // 
            // mResolutionComboBox
            // 
            this.mResolutionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mResolutionComboBox.Name = "mResolutionComboBox";
            this.mResolutionComboBox.Size = new System.Drawing.Size(121, 25);
            this.mResolutionComboBox.SelectedIndexChanged += new System.EventHandler(this.mResolutionComboBox_SelectedIndexChanged);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(42, 22);
            this.toolStripLabel1.Text = "Zoom:";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel2,
            this.mResolutionComboBox,
            this.toolStripLabel1,
            this.mZoomTextBox,
            this.toolStripLabel3,
            this.mSafeDisplayComboBox,
            this.mBoundsToolStripDropDownButton,
            this.mShowGridToolStripButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(986, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // mZoomTextBox
            // 
            this.mZoomTextBox.Name = "mZoomTextBox";
            this.mZoomTextBox.Size = new System.Drawing.Size(75, 25);
            this.mZoomTextBox.Text = "100%";
            this.mZoomTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.mZoomTextBox_KeyDown);
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(73, 22);
            this.toolStripLabel3.Text = "Safe Display:";
            // 
            // mSafeDisplayComboBox
            // 
            this.mSafeDisplayComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mSafeDisplayComboBox.Name = "mSafeDisplayComboBox";
            this.mSafeDisplayComboBox.Size = new System.Drawing.Size(121, 25);
            this.mSafeDisplayComboBox.SelectedIndexChanged += new System.EventHandler(this.mSafeDisplayComboBox_SelectedIndexChanged);
            // 
            // mBoundsToolStripDropDownButton
            // 
            this.mBoundsToolStripDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.mBoundsToolStripDropDownButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hideScreenBoundsToolStripMenuItem,
            this.showScreenBoundsRedToolStripMenuItem,
            this.showScreenBoundsTransparentToolStripMenuItem});
            this.mBoundsToolStripDropDownButton.Image = global::Otter.Editor.Properties.Resources.NoBounds;
            this.mBoundsToolStripDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mBoundsToolStripDropDownButton.Name = "mBoundsToolStripDropDownButton";
            this.mBoundsToolStripDropDownButton.Size = new System.Drawing.Size(29, 22);
            this.mBoundsToolStripDropDownButton.Text = "toolStripDropDownButton1";
            this.mBoundsToolStripDropDownButton.ToolTipText = "Control boundary render options";
            // 
            // hideScreenBoundsToolStripMenuItem
            // 
            this.hideScreenBoundsToolStripMenuItem.Image = global::Otter.Editor.Properties.Resources.NoBounds;
            this.hideScreenBoundsToolStripMenuItem.Name = "hideScreenBoundsToolStripMenuItem";
            this.hideScreenBoundsToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.hideScreenBoundsToolStripMenuItem.Text = "None";
            this.hideScreenBoundsToolStripMenuItem.ToolTipText = "Areas of controls rendered outside of the screen bounds are rendered normally";
            this.hideScreenBoundsToolStripMenuItem.Click += new System.EventHandler(this.showScreenBoundsToolStripMenuItem_Click);
            // 
            // showScreenBoundsRedToolStripMenuItem
            // 
            this.showScreenBoundsRedToolStripMenuItem.Image = global::Otter.Editor.Properties.Resources.RedBounds;
            this.showScreenBoundsRedToolStripMenuItem.Name = "showScreenBoundsRedToolStripMenuItem";
            this.showScreenBoundsRedToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.showScreenBoundsRedToolStripMenuItem.Text = "Red";
            this.showScreenBoundsRedToolStripMenuItem.ToolTipText = "Areas of controls rendered outside of the screen bounds are rendered with a red c" +
                "olor";
            this.showScreenBoundsRedToolStripMenuItem.Click += new System.EventHandler(this.showScreenBoundsToolStripMenuItem_Click);
            // 
            // showScreenBoundsTransparentToolStripMenuItem
            // 
            this.showScreenBoundsTransparentToolStripMenuItem.Image = global::Otter.Editor.Properties.Resources.TransBounds;
            this.showScreenBoundsTransparentToolStripMenuItem.Name = "showScreenBoundsTransparentToolStripMenuItem";
            this.showScreenBoundsTransparentToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.showScreenBoundsTransparentToolStripMenuItem.Text = "Transparent";
            this.showScreenBoundsTransparentToolStripMenuItem.ToolTipText = "Areas of controls rendered outside of the screen bounds are rendered semi-transpa" +
                "rent";
            this.showScreenBoundsTransparentToolStripMenuItem.Click += new System.EventHandler(this.showScreenBoundsToolStripMenuItem_Click);
            // 
            // mShowGridToolStripButton
            // 
            this.mShowGridToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.mShowGridToolStripButton.Image = global::Otter.Editor.Properties.Resources.Grid;
            this.mShowGridToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mShowGridToolStripButton.Name = "mShowGridToolStripButton";
            this.mShowGridToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.mShowGridToolStripButton.Text = "toolStripButton1";
            this.mShowGridToolStripButton.Click += new System.EventHandler(this.mShowGridToolStripButton_Click);
            // 
            // mRenderPanel
            // 
            this.mRenderPanel.AllowDrop = true;
            this.mRenderPanel.BackColor = System.Drawing.Color.Black;
            this.mRenderPanel.ContextMenuStrip = this.mContextMenuStrip;
            this.mRenderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mRenderPanel.Location = new System.Drawing.Point(0, 25);
            this.mRenderPanel.Name = "mRenderPanel";
            this.mRenderPanel.Size = new System.Drawing.Size(986, 762);
            this.mRenderPanel.TabIndex = 0;
            this.mRenderPanel.Load += new System.EventHandler(this.mRenderPanel_Load);
            this.mRenderPanel.Disposed += new System.EventHandler(this.mRenderPanel_Disposed);
            this.mRenderPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.mRenderPanel_Paint);
            this.mRenderPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mRenderPanel_MouseMove);
            this.mRenderPanel.KeyUp += new System.Windows.Forms.KeyEventHandler(this.mRenderPanel_KeyUp);
            this.mRenderPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mRenderPanel_MouseDown);
            this.mRenderPanel.Resize += new System.EventHandler(this.mRenderPanel_Resize);
            this.mRenderPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.mRenderPanel_MouseUp);
            this.mRenderPanel.KeyDown += new System.Windows.Forms.KeyEventHandler(this.mRenderPanel_KeyDown);
            this.mRenderPanel.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(mRenderPanel_PreviewKeyDown);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(158, 6);
            // 
            // mCustomActionsToolStripMenuItem
            // 
            this.mCustomActionsToolStripMenuItem.Name = "mCustomActionsToolStripMenuItem";
            this.mCustomActionsToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.mCustomActionsToolStripMenuItem.Text = "Custom Actions";
            // 
            // SceneView
            // 
            this.AllowEndUserDocking = false;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(986, 787);
            this.Controls.Add(this.mRenderPanel);
            this.Controls.Add(this.toolStrip1);
            this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "SceneView";
            this.TabText = "Scene";
            this.Text = "Scene";
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.SceneView_MouseWheel);
            this.mContextMenuStrip.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.ContextMenuStrip mContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem mDeleteStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mBringToFrontToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mSendToBackToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem mCreateKeyFrameToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem DuplicateStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripComboBox mResolutionComboBox;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripTextBox mZoomTextBox;
        private System.Windows.Forms.ToolStripDropDownButton mBoundsToolStripDropDownButton;
        private System.Windows.Forms.ToolStripMenuItem hideScreenBoundsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showScreenBoundsRedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showScreenBoundsTransparentToolStripMenuItem;
        private CustomControls.RenderPanel mRenderPanel;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripComboBox mSafeDisplayComboBox;
        private System.Windows.Forms.ToolStripButton mShowGridToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem mCustomActionsToolStripMenuItem;

    }
}