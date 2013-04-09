using System;
using System.Drawing;
using System.Xml;
using System.Xml.Serialization;
using Otter.Export;
using System.ComponentModel;

namespace Otter.UI.Animation
{
    public class KeyFrame : BaseFrame, IExportable
    {
        #region Enums
        public enum Ease
        {
            None,
            EaseIn,
            EaseOut,
            EaseInOut
        }
        #endregion

        #region Data
        private Ease mEaseType = 0;
        private int mEaseAmount = 0;

        private AnchorData mLeftAnchor = new AnchorData();
        private AnchorData mRightAnchor = new AnchorData();
        private AnchorData mTopAnchor = new AnchorData();
        private AnchorData mBottomAnchor = new AnchorData();

        private ControlLayout mLayout = null;
        #endregion

        #region Properties
        /// <summary>
        /// Returns a unique fourcc identifier for the keyframe
        /// </summary>
        protected virtual char[] FourCC
        {
            get
            {
                return new char[]{'K', 'F', 'R', 'M'};
            }
        }

        /// <summary>
        /// Control's layout
        /// </summary>
        [Browsable(false)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public ControlLayout Layout
        {
            get { return mLayout; }
            set
            {
                mLayout = value; 
            }
        }

        /// <summary>
        /// Gets / Sets the keyframe ease in
        /// </summary>
        [XmlAttribute]
        [Category("Ease")]
        [DisplayName("Function")]
        public Ease EaseFunction
        {
            get { return mEaseType; }
            set 
            {
                mEaseType = value;
            }
        }

        /// <summary>
        /// Gets / Sets the keyframe ease out
        /// </summary>
        [XmlAttribute]
        [Category("Ease")]
        [DisplayName("Amount")]
        public int EaseAmount
        {
            get { return mEaseAmount; }
            set
            {
                mEaseAmount = (value < 0 ? 0 : (value > 100 ? 100 : value)); 
            }
        }

        /// <summary>
        /// Gets / Sets the left anchor data
        /// </summary>
        [Browsable(false)]
        public AnchorData LeftAnchor
        {
            get { return mLeftAnchor; }
            set { mLeftAnchor = value; }
        }

        /// <summary>
        /// Gets / Sets the right anchor data
        /// </summary>
        [Browsable(false)]
        public AnchorData RightAnchor
        {
            get { return mRightAnchor; }
            set { mRightAnchor = value; }
        }

        /// <summary>
        /// Gets / Sets the top anchor data
        /// </summary>
        [Browsable(false)]
        public AnchorData TopAnchor
        {
            get { return mTopAnchor; }
            set { mTopAnchor = value; }
        }

        /// <summary>
        /// Gets / Sets the bottom anchor data
        /// </summary>
        [Browsable(false)]
        public AnchorData BottomAnchor
        {
            get { return mBottomAnchor; }
            set { mBottomAnchor = value; }
        }
        #endregion

        /// <summary>
        /// Default constructor, necessary for serialization
        /// </summary>
        public KeyFrame()
        {
            mLayout = new ControlLayout();
        }

        /// <summary>
        /// Constructs a keyframe with the specified layout.
        /// </summary>
        /// <param name="layout"></param>
        public KeyFrame(ControlLayout layout)
        {
            mLayout = layout;
        }

        /// <summary>
        /// Clones this keyframe
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            KeyFrame keyframe = new KeyFrame();
            keyframe.Frame = this.Frame;
            keyframe.Layout = this.Layout.Clone() as ControlLayout;
            keyframe.LeftAnchor = (AnchorData)this.LeftAnchor.Clone();
            keyframe.RightAnchor = (AnchorData)this.RightAnchor.Clone();
            keyframe.TopAnchor = (AnchorData)this.TopAnchor.Clone();
            keyframe.BottomAnchor = (AnchorData)this.BottomAnchor.Clone();
            keyframe.EaseFunction = this.EaseFunction;
            keyframe.EaseAmount = this.EaseAmount;

            return keyframe;
        }

        /// <summary>
        /// Exports this frame to a binary stream
        /// </summary>
        /// <param name="bw"></param>
        public virtual void Export(PlatformBinaryWriter bw)
        {
            FourCCStack fourCCStack = new FourCCStack(bw);
            fourCCStack.Push("KFRM");
            {
                bw.Write(this.Frame);

                bw.Write((int)this.EaseFunction);
                bw.Write(this.EaseAmount);

                bw.Write(this.LeftAnchor.AbsoluteValue);
                bw.Write(this.LeftAnchor.RatioValue);

                bw.Write(this.RightAnchor.AbsoluteValue);
                bw.Write(this.RightAnchor.RatioValue);

                bw.Write(this.TopAnchor.AbsoluteValue);
                bw.Write(this.TopAnchor.RatioValue);

                bw.Write(this.BottomAnchor.AbsoluteValue);
                bw.Write(this.BottomAnchor.RatioValue);

                this.Layout.Export(bw);
            }
            
            fourCCStack.Pop();
        }
    }
}
