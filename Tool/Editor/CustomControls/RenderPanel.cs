using System;
using System.Collections.Generic;
using System.Text;

using System.Windows.Forms;

namespace Otter.Editor.CustomControls
{
    /// <summary>
    /// 
    /// </summary>
    public class RenderPanel : UserControl
    {
        #region Data
        private long mRenderContext = 0;
        #endregion

        #region Properties
        /// <summary>
        /// Retrieves the panel's render context
        /// </summary>
        public long RenderContext
        {
            get { return mRenderContext; }
        }
        #endregion

        public RenderPanel()
        {
            if (!this.DesignMode)
            {
                this.Resize += new EventHandler(RenderPanel_Resize);
                this.Disposed += new EventHandler(RenderPanel_Disposed);
                this.HandleCreated += new EventHandler(RenderPanel_HandleCreated);
            }
        }

        void RenderPanel_HandleCreated(object sender, EventArgs e)
        {
            if (this.DesignMode)
                return;

            Otter.Interface.Graphics.Instance.DestroyContext((int)mRenderContext);
            mRenderContext = Otter.Interface.Graphics.Instance.CreateContext(this.Handle.ToInt32(), this.Size.Width, this.Size.Height);
        }

        protected override void OnBindingContextChanged(EventArgs e)
        {
            base.OnBindingContextChanged(e);
        }

        protected override void OnNotifyMessage(Message m)
        {
            System.Console.WriteLine("Message: " + m);
            base.OnNotifyMessage(m);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (this.DesignMode)
                return;

            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.Opaque, true);
        }

        protected override bool IsInputKey(Keys keyData)
        {
            if (keyData == Keys.Left ||
                keyData == Keys.Right ||
                keyData == Keys.Up ||
                keyData == Keys.Down ||
                keyData == Keys.Shift ||
                keyData == Keys.ShiftKey)
            {
                return true;
            }

            return base.IsInputKey(keyData);
        }

        void RenderPanel_Disposed(object sender, EventArgs e)
        {
            if (this.DesignMode)
                return;

            Otter.Interface.Graphics.Instance.DestroyContext((int)mRenderContext);
        }

        void RenderPanel_Resize(object sender, EventArgs e)
        {
            if (this.DesignMode)
                return;

            Otter.Interface.Graphics.Instance.DestroyContext((int)mRenderContext);

            mRenderContext = Otter.Interface.Graphics.Instance.CreateContext(this.Handle.ToInt32(), this.Size.Width, this.Size.Height);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // RenderPanel
            // 
            this.DoubleBuffered = true;
            this.Name = "RenderPanel";
            this.ResumeLayout(false);

        }
    }
}
