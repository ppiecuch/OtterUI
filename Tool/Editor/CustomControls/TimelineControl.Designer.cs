namespace Otter.Editor.CustomControls
{
    partial class TimelineControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.mContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mClearFramesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mCopyFramesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mPasteFramesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.mLoopFramesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mClearLoopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mInsertFramesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mTrimFramesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.mEventsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mHScrollBar = new System.Windows.Forms.HScrollBar();
            this.mVScrollBar = new System.Windows.Forms.VScrollBar();
            this.label1 = new System.Windows.Forms.Label();
            this.mContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // mContextMenuStrip
            // 
            this.mContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mClearFramesToolStripMenuItem,
            this.mCopyFramesToolStripMenuItem,
            this.mPasteFramesToolStripMenuItem,
            this.toolStripMenuItem1,
            this.mLoopFramesToolStripMenuItem,
            this.mClearLoopToolStripMenuItem,
            this.toolStripSeparator1,
            this.mInsertFramesToolStripMenuItem,
            this.mTrimFramesToolStripMenuItem,
            this.toolStripMenuItem2,
            this.mEventsToolStripMenuItem});
            this.mContextMenuStrip.Name = "contextMenuStrip1";
            this.mContextMenuStrip.Size = new System.Drawing.Size(169, 220);
            this.mContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.mContextMenuStrip_Opening);
            // 
            // mClearFramesToolStripMenuItem
            // 
            this.mClearFramesToolStripMenuItem.Name = "mClearFramesToolStripMenuItem";
            this.mClearFramesToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.mClearFramesToolStripMenuItem.Text = "Clear Keyframe(s)";
            this.mClearFramesToolStripMenuItem.Click += new System.EventHandler(this.mClearFramesToolStripMenuItem_Click);
            // 
            // mCopyFramesToolStripMenuItem
            // 
            this.mCopyFramesToolStripMenuItem.Name = "mCopyFramesToolStripMenuItem";
            this.mCopyFramesToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.mCopyFramesToolStripMenuItem.Text = "Copy Keyframe(s)";
            this.mCopyFramesToolStripMenuItem.Click += new System.EventHandler(this.mCopyFramesToolStripMenuItem_Click);
            // 
            // mPasteFramesToolStripMenuItem
            // 
            this.mPasteFramesToolStripMenuItem.Name = "mPasteFramesToolStripMenuItem";
            this.mPasteFramesToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.mPasteFramesToolStripMenuItem.Text = "Paste Keyframe(s)";
            this.mPasteFramesToolStripMenuItem.Click += new System.EventHandler(this.mPasteFramesToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(165, 6);
            // 
            // mLoopFramesToolStripMenuItem
            // 
            this.mLoopFramesToolStripMenuItem.Name = "mLoopFramesToolStripMenuItem";
            this.mLoopFramesToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.mLoopFramesToolStripMenuItem.Text = "Loop Frames";
            this.mLoopFramesToolStripMenuItem.Click += new System.EventHandler(this.mLoopFramesToolStripMenuItem_Click);
            // 
            // mClearLoopToolStripMenuItem
            // 
            this.mClearLoopToolStripMenuItem.Name = "mClearLoopToolStripMenuItem";
            this.mClearLoopToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.mClearLoopToolStripMenuItem.Text = "Clear Loop";
            this.mClearLoopToolStripMenuItem.Click += new System.EventHandler(this.mClearLoopToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(165, 6);
            // 
            // mInsertFramesToolStripMenuItem
            // 
            this.mInsertFramesToolStripMenuItem.Name = "mInsertFramesToolStripMenuItem";
            this.mInsertFramesToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.mInsertFramesToolStripMenuItem.Text = "Insert Frames";
            this.mInsertFramesToolStripMenuItem.Click += new System.EventHandler(this.mInsertFramesToolStripMenuItem_Click);
            // 
            // mTrimFramesToolStripMenuItem
            // 
            this.mTrimFramesToolStripMenuItem.Name = "mTrimFramesToolStripMenuItem";
            this.mTrimFramesToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.mTrimFramesToolStripMenuItem.Text = "Trim Frames";
            this.mTrimFramesToolStripMenuItem.Click += new System.EventHandler(this.mTrimFramesToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(165, 6);
            // 
            // mEventsToolStripMenuItem
            // 
            this.mEventsToolStripMenuItem.Name = "mEventsToolStripMenuItem";
            this.mEventsToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.mEventsToolStripMenuItem.Text = "Actions...";
            this.mEventsToolStripMenuItem.Click += new System.EventHandler(this.mActionsToolStripMenuItem_Click);
            // 
            // mHScrollBar
            // 
            this.mHScrollBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mHScrollBar.Enabled = false;
            this.mHScrollBar.Location = new System.Drawing.Point(0, 345);
            this.mHScrollBar.Name = "mHScrollBar";
            this.mHScrollBar.Size = new System.Drawing.Size(868, 16);
            this.mHScrollBar.TabIndex = 1;
            this.mHScrollBar.ValueChanged += new System.EventHandler(this.mHScrollBar_ValueChanged);
            // 
            // mVScrollBar
            // 
            this.mVScrollBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mVScrollBar.Enabled = false;
            this.mVScrollBar.Location = new System.Drawing.Point(868, 0);
            this.mVScrollBar.Name = "mVScrollBar";
            this.mVScrollBar.Size = new System.Drawing.Size(16, 345);
            this.mVScrollBar.TabIndex = 2;
            this.mVScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.mVScrollBar_Scroll);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.BackColor = System.Drawing.SystemColors.Control;
            this.label1.Location = new System.Drawing.Point(868, 345);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(16, 16);
            this.label1.TabIndex = 3;
            // 
            // TimelineControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ContextMenuStrip = this.mContextMenuStrip;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.mVScrollBar);
            this.Controls.Add(this.mHScrollBar);
            this.DoubleBuffered = true;
            this.Name = "TimelineControl";
            this.Size = new System.Drawing.Size(884, 361);
            this.Load += new System.EventHandler(this.TimelineControl_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TimelineControl_KeyUp);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TimelineControl_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TimelineControl_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TimelineControl_MouseUp);
            this.mContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip mContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem mClearFramesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem mLoopFramesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mClearLoopToolStripMenuItem;
        private System.Windows.Forms.HScrollBar mHScrollBar;
        private System.Windows.Forms.VScrollBar mVScrollBar;
        private System.Windows.Forms.ToolStripMenuItem mCopyFramesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mPasteFramesToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem mInsertFramesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mTrimFramesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem mEventsToolStripMenuItem;
    }
}
