using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Drawing.Design;

using System.IO;
using Otter.Export;
using Otter.UI.Animation;
using Otter.Interface;
using Otter.TypeEditors;

namespace Otter.UI
{
    public delegate void PointDelegate(PointF oldLoc, PointF newLoc);
    public delegate void SizeDelegate(SizeF oldSize, SizeF newSize);
    public delegate void FloatDelegate(float oldValue, float newValue);

    /// <summary>
    /// Enum indicating what 'area' of a control 
    /// a point is in.
    /// </summary>
    [Flags]
    public enum PointArea
    {
        None        = 0,
        Body        = 1 << 0,
        Top         = 1 << 1,
        Bottom      = 1 << 2,
        Left        = 1 << 3,
        Right       = 1 << 4,
        Center      = 1 << 5
    }

    /// <summary>
    /// Maintains the layout data for a control (size, position, etc)
    /// </summary>
    public class ControlLayout : ICloneable
    {
        #region Events and Delegates
        public event PointDelegate LocationChanged = null;
        public event PointDelegate CenterChanged = null;
        public event SizeDelegate SizeChanged = null;
        public event FloatDelegate RotationChanged = null;
        #endregion

        #region Data
        private PointF mLocation = Point.Empty;
        private PointF mCenter = PointF.Empty;
        private SizeF mSize = SizeF.Empty;
        private float mRotation = 0.0f;
        #endregion

        #region Properties
        /// <summary>
        /// Location of the control
        /// </summary>
        [TypeConverter(typeof(Otter.TypeConverters.PointFConverter))]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Layout")]
        public PointF Location
        {
            get { return mLocation; }
            set
            {
                if (mLocation != value)
                {
                    PointF oldLoc = mLocation;
                    PointF newLoc = value;

                    mLocation = value;

                    if (LocationChanged != null)
                        LocationChanged(oldLoc, newLoc);
                }
            }
        }

        /// <summary>
        /// Center of the control, effectively its origin.
        /// </summary>
        [Browsable(false)]
        public PointF Center
        {
            get { return mCenter; }
            set 
            {
                if (mCenter != value)
                {
                    PointF oldValue = mCenter;
                    PointF newValue = value;

                    mCenter = value;

                    if (CenterChanged != null)
                        CenterChanged(oldValue, newValue);
                }
            }
        }

        /// <summary>
        /// Size of the control
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Layout")]
        public SizeF Size
        {
            get { return mSize; }
            set
            {
                if (mSize != value)
                {
                    SizeF oldSize = mSize;
                    SizeF newSize = value;

                    mSize = value;

                    if (SizeChanged != null)
                        SizeChanged(oldSize, newSize);
                }
            }
        }

        /// <summary>
        /// SelectedControl's rotation
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Layout")]
        public float Rotation
        {
            get { return mRotation; }
            set
            {
                if (mRotation != value)
                {
                    float oldValue = mRotation;
                    float newValue = value;

                    mRotation = value;

                    if (RotationChanged != null)
                        RotationChanged(oldValue, newValue);
                }
            }
        }

        /// <summary>
        /// Gets / Sets the left bound
        /// </summary>
        [XmlIgnore]
        [Category("Bounds")]
        public float Left
        {
            get
            {
                return -Center.X;
            }
            set
            {
                float oldRight = Right;
                Center = new PointF(-value, Center.Y);
                Size = new SizeF(oldRight - value, Size.Height);
            }
        }

        /// <summary>
        /// Gets / Sets the right bound
        /// </summary>
        [XmlIgnore]
        [Category("Bounds")]
        public float Right
        {
            get
            {
                return Left + Size.Width;
            }
            set
            {
                Size = new SizeF(value - Left, Size.Height);
            }
        }

        /// <summary>
        /// Gets / Sets the top bound
        /// </summary>
        [XmlIgnore]
        [Category("Bounds")]
        public float Top
        {
            get
            {
                return -Center.Y;
            }
            set
            {
                float oldBottom = Bottom;
                Center = new PointF(Center.X, -value);
                Size = new SizeF(Size.Width, oldBottom - value);
            }
        }

        /// <summary>
        /// Gets / Sets the bottom bound
        /// </summary>
        [XmlIgnore]
        [Category("Bounds")]
        public float Bottom
        {
            get
            {
                return Top + Size.Height;
            }
            set
            {
                Size = new SizeF(Size.Width, value - Top);
            }
        }
        #endregion

        /// <summary>
        /// Sets the base attributes from a source layout
        /// </summary>
        /// <param name="layout"></param>
        protected void SetFrom(ControlLayout layout)
        {
            this.Location = layout.Location;
            this.Center = layout.Center;
            this.Rotation = layout.Rotation;
            this.Size = layout.Size;
        }

        /// <summary>
        /// Clones the object
        /// </summary>
        /// <returns></returns>
        public virtual object Clone()
        {
            ControlLayout layout = new ControlLayout();
            layout.SetFrom(this);
            return layout;
        }
        
        /// <summary>
        /// Exports the base data
        /// </summary>
        /// <param name="bw"></param>
        protected void ExportBase(PlatformBinaryWriter bw)
        {
            bw.Write(this.Center.X);
            bw.Write(this.Center.Y);

            bw.Write(this.Location.X);
            bw.Write(this.Location.Y);

            bw.Write(this.Size.Width);
            bw.Write(this.Size.Height);

            bw.Write(this.Rotation);
        }

        /// <summary>
        /// Exports the layout to binary format
        /// </summary>
        /// <param name="bw"></param>
        public virtual void Export(PlatformBinaryWriter bw)
        {
            FourCCStack fourCCStack = new FourCCStack(bw);
            fourCCStack.Push("CLLT");
            {
                ExportBase(bw);
            }
            fourCCStack.Pop();
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
            if ((obj is ControlLayout))
                return base.Equals(obj);

            ControlLayout layout = (ControlLayout)obj;
            return (layout.Location.Equals(this.Location) &&
                    layout.Center.Equals(this.Center) &&
                    layout.Size.Equals(this.Size) &&
                    layout.Rotation == this.Rotation);
        }
    }

    /// <summary>
    /// Base GUI SelectedControl class.
    /// </summary>
    public abstract class GUIControl : IExportable, ICloneable, IDisposable
    {
        #region Events and Delegates
        public event PointDelegate LocationChanged = null;
        public event SizeDelegate SizeChanged = null;
        public event FloatDelegate RotationChanged = null;
        #endregion

        #region Data
        private int mID = -1;
        private string mName = "";

        private GUIControl mParent = null;
        private GUIScene mScene = null;
        private GUIControlCollection mControls = new GUIControlCollection();

        private AnchorFlags mAnchorFlags = AnchorFlags.None;

        private ControlLayout mControlLayout = new ControlLayout();

        private Matrix mTransform = Matrix.Identity;
        private Matrix mTransformInv = Matrix.Identity;
        private Matrix mFullTransform = Matrix.Identity;
        private Matrix mFullTransformInv = Matrix.Identity;

        private bool mUpdateTransform = true;
        private bool mUpdateFullTransform = true;
        private bool mLocked;
        private bool mHidden;

        private int mMaskID = -1;
        #endregion

        #region Properties
        /// <summary>
        /// Gets / Sets the control's ID
        /// </summary>
        [ReadOnly(true)]
        [XmlAttribute]
        [Category("General")]
        public int ID
        {
            get { return mID; }
            set { mID = value; }
        }

        /// <summary>
        /// Gets/Sets the name of the control
        /// </summary>
        [XmlAttribute]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("General")]
        public string Name
        {
            get { return mName; }
            set { mName = value; }
        }

        /// <summary>
        /// Gets / Sets the control's anchor flags
        /// </summary>
        [Editor(typeof(UIAnchorEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(Otter.TypeConverters.AnchorConverter))]
        [Category("Layout")]
        [DisplayName("Anchors")]
        public AnchorFlags AnchorFlags
        {
            get { return mAnchorFlags; }
            set { mAnchorFlags = value; }
        }

        /// <summary>
        /// Gets / Sets the framedata
        /// </summary>
        [Browsable(false)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public ControlLayout Layout
        {
            get { return mControlLayout; }
            set 
            {
                if (mControlLayout != value)
                {
                    if (mControlLayout != null)
                    {
                        mControlLayout.LocationChanged -= new PointDelegate(Layout_LocationChanged);
                        mControlLayout.SizeChanged -= new SizeDelegate(Layout_SizeChanged);
                        mControlLayout.CenterChanged -= new PointDelegate(Layout_CenterChanged);
                        mControlLayout.RotationChanged -= new FloatDelegate(Layout_RotationChanged);
                    }

                    mControlLayout = value;

                    if (mControlLayout != null)
                    {
                        mControlLayout.LocationChanged += new PointDelegate(Layout_LocationChanged);
                        mControlLayout.SizeChanged += new SizeDelegate(Layout_SizeChanged);
                        mControlLayout.CenterChanged += new PointDelegate(Layout_CenterChanged);
                        mControlLayout.RotationChanged += new FloatDelegate(Layout_RotationChanged);
                    }

                    mUpdateFullTransform = true;
                    mUpdateTransform = true;
                }
            }
        }
        
        /// <summary>
        /// Location of the control
        /// </summary>
        [TypeConverter(typeof(Otter.TypeConverters.PointFConverter))]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Layout")]
        [XmlIgnore()]
        public PointF Location
        {
            get { return Layout.Location; }
            set 
            { 
                Layout.Location = value;
            }
        }

        /// <summary>
        /// Center of the control, effectively its origin.
        /// </summary>
        [Browsable(false)]
        [XmlIgnore()]
        public PointF Center
        {
            get { return Layout.Center; }
            set
            { 
                Layout.Center = value;
            }
        }

        /// <summary>
        /// Size of the control
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Layout")]
        [XmlIgnore()]
        public SizeF Size
        {
            get { return Layout.Size; }
            set 
            {
                Layout.Size = value;
            }
        }

        /// <summary>
        /// SelectedControl's rotation
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Layout")]
        [XmlIgnore()]
        public float Rotation
        {
            get { return Layout.Rotation; }
            set 
            {
                Layout.Rotation = value;
            }
        }

        /// <summary>
        /// Gets / Sets the control's mask
        /// </summary>
        [XmlAttribute]
        [Category("General")]
        [TypeConverter(typeof(Otter.TypeConverters.MaskConverter))]
        [Editor(typeof(Otter.TypeEditors.UIMaskEditor), typeof(UITypeEditor))]
        public int Mask
        {
            get { return mMaskID; }
            set { mMaskID = value; }
        }

        /// <summary>
        /// Gets the most-specific mask inherited by this control
        /// </summary>
        [XmlAttribute]
        [Browsable(false)]
        public int InheritedMask
        {
            get
            {
                if (mMaskID != -1 || mParent == null)
                    return mMaskID;
                else
                    return mParent.InheritedMask;
            }
        }

        /// <summary>
        /// Gets / Sets the left bound
        /// </summary>
        [XmlIgnore]
        [Category("Bounds")]
        public float Left
        {
            get { return Layout.Left; }
            set
            {
                Layout.Left = value;
            }
        }

        /// <summary>
        /// Gets / Sets the right bound
        /// </summary>
        [XmlIgnore]
        [Category("Bounds")]
        public float Right
        {
            get { return Layout.Right; }
            set 
            {
                Layout.Right = value;
            }
        }

        /// <summary>
        /// Gets / Sets the top bound
        /// </summary>
        [XmlIgnore]
        [Category("Bounds")]
        public float Top
        {
            get { return Layout.Top; }
            set
            {
                Layout.Top = value;
            }
        }

        /// <summary>
        /// Gets / Sets the bottom bound
        /// </summary>
        [XmlIgnore]
        [Category("Bounds")]
        public float Bottom
        {
            get { return Layout.Bottom; }
            set
            {
                Layout.Bottom = value;
            }
        }

        /// <summary>
        /// Gets the control's local transform
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public Matrix Transform
        {
            get 
            {
                UpdateTransform();
                return mTransform;
            }
        }

        [XmlIgnore]
        [Browsable(false)]
        public Matrix TransformInv
        {
            get
            {
                UpdateTransform(); 
                return mTransformInv;
            }
        }

        /// <summary>
        /// Gets the control's full transform, with the parent's concatenated
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public Matrix FullTransform
        {
            get
            {
                UpdateFullTransform();
                return mFullTransform;
            }
        }

        [XmlIgnore]
        [Browsable(false)]
        public Matrix FullTransformInv
        {
            get
            {
                UpdateFullTransform();
                return mFullTransformInv; 
            }
        }

        /// <summary>
        /// Gets / Sets the list of child controls.
        /// </summary>
        [Browsable(false)]
        public GUIControlCollection Controls
        {
            get 
            { 
                return mControls; 
            }
            set
            { 
                mControls = value;
            }
        }

        /// <summary>
        /// Gets / Sets the parent control
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public GUIControl Parent
        {
            get { return mParent; }
            set 
            {
                if (mParent != value)
                {
                    if (mParent != null)
                    {
                        mParent.LocationChanged -= new PointDelegate(Parent_LocationChanged);
                        mParent.SizeChanged -= new SizeDelegate(Parent_SizeChanged);
                        mParent.RotationChanged -= new FloatDelegate(value_RotationChanged);
                    }

                    mParent = value;

                    if (mParent != null)
                    {
                        value.LocationChanged += new PointDelegate(Parent_LocationChanged);
                        value.SizeChanged += new SizeDelegate(Parent_SizeChanged);
                        value.RotationChanged += new FloatDelegate(value_RotationChanged);
                    }

                    mUpdateFullTransform = true;
                }
            }
        }

        /// <summary>
        /// Retrieves the parent view
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public GUIView ParentView
        {
            get
            {
                if (mParent is GUIView)
                    return mParent as GUIView;

                return (mParent != null) ? mParent.ParentView : null;
            }
        }

        /// <summary>
        /// Gets / Sets the parent scene for this control
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public virtual GUIScene Scene
        {
            get { return mScene; }
            set
            {
                if (mScene != value)
                {
                    mScene = value;

                    // What did this accomplish?!
                    // if (mScene != null)
                    //    this.Size = new SizeF(Scene.Resolution.Width, Scene.Resolution.Height);

                    foreach (GUIControl control in Controls)
                        control.Scene = mScene;
                }
            }
        }

        /// <summary>
        /// Gets the list of used textures
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public virtual List<int> TextureIDs
        {
            get
            {
                return new List<int>();
            }
        }

        /// <summary>
        /// Gets the list of used sounds
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public virtual List<int> SoundIDs
        {
            get
            {
                return new List<int>();
            }
        }

        /// <summary>
        /// Allows / disallows movement of control
        /// </summary> 
        [XmlAttribute]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("General")]
        public bool Locked
        {
            get { return mLocked; }
            set { mLocked = value; }
        }

        /// <summary>
        /// Shows / hides the control
        /// </summary> 
        [XmlAttribute]
        [Category("General")]
        public bool Hidden
        {
            get { return mHidden; }
            set { mHidden = value; }
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Called when the FrameData's size has changed.  Propogate the event forward
        /// </summary>
        /// <param name="oldSize"></param>
        /// <param name="newSize"></param>
        void Layout_SizeChanged(SizeF oldSize, SizeF newSize)
        {
            mUpdateTransform = true;
            mUpdateFullTransform = true;

            if (SizeChanged != null)
                SizeChanged(oldSize, newSize);
        }

        /// <summary>
        /// Called when the FrameData's location has changed.  Propogate the event forwards
        /// </summary>
        /// <param name="oldLoc"></param>
        /// <param name="newLoc"></param>
        void Layout_LocationChanged(PointF oldLoc, PointF newLoc)
        {
            mUpdateTransform = true;
            mUpdateFullTransform = true;

            if (LocationChanged != null)
                LocationChanged(oldLoc, newLoc);
        }

        /// <summary>
        /// Called when the layet's center has changed
        /// </summary>
        /// <param name="oldLoc"></param>
        /// <param name="newLoc"></param>
        void Layout_CenterChanged(PointF oldLoc, PointF newLoc)
        {
            mUpdateTransform = true;
            mUpdateFullTransform = true;
        }

        /// <summary>
        /// Called when the layout's rotation has changed
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        void Layout_RotationChanged(float oldValue, float newValue)
        {
            mUpdateTransform = true;
            mUpdateFullTransform = true;
        }

        /// <summary>
        /// Called whenever a child control is added
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="control"></param>
        void Controls_OnControlAdded(object sender, GUIControl control)
        {
            control.Parent = this;
        }

        /// <summary>
        /// Called whenever a child control is removed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="control"></param>
        void Controls_OnControlRemoved(object sender, GUIControl control)
        {
            if (control.Parent == this)
                control.Parent = null;
        }

        /// <summary>
        /// Called when our parent's size has changed.
        /// </summary>
        /// <param name="oldLoc"></param>
        /// <param name="newLoc"></param>
        void Parent_LocationChanged(PointF oldLoc, PointF newLoc)
        {
            mUpdateTransform = true;
            mUpdateFullTransform = true;
        }

        /// <summary>
        /// Called when the parent's size has changed.  Recompute this control's layout
        /// based on anchor flags and related data.
        /// </summary>
        /// <param name="oldSize"></param>
        /// <param name="newSize"></param>
        void Parent_SizeChanged(SizeF oldSize, SizeF newSize)
        {
            float ratio = this.Size.Height != 0 ? (this.Size.Width / this.Size.Height) : 0.0f;

            float deltaW = newSize.Width - oldSize.Width;
            float deltaH = newSize.Height - oldSize.Height;

            float left = this.Location.X - this.Center.X;
            float right = left + this.Size.Width;
            float top = this.Location.Y - this.Center.Y;
            float bottom = top + this.Size.Height;

            RectangleF controlRect = new RectangleF(left, top, (right - left), (bottom - top));
            PointF center = Layout.Center;
            RectangleF parentRect = new RectangleF(0, 0, newSize.Width, newSize.Height);

            uint flags = (uint)AnchorFlags;
            Otter.Interface.Utilities.ComputeAnchoredRectangle( ref controlRect, 
                                                                ref center, 
                                                                ref parentRect, 
                                                                flags, 
                                                                (AnchorFlags & AnchorFlags.LeftRelative) == 0 ? (left) : (left / oldSize.Width),
                                                                (AnchorFlags & AnchorFlags.RightRelative) == 0 ? (oldSize.Width - right) : (right / oldSize.Width),
                                                                (AnchorFlags & AnchorFlags.TopRelative) == 0 ? (top) : (top / oldSize.Height), 
                                                                (AnchorFlags & AnchorFlags.BottomRelative) == 0 ? (oldSize.Height - bottom) : (bottom / oldSize.Height));

            this.Center = center;
            this.Location = new PointF(controlRect.Left + center.X, controlRect.Top + center.Y);            
            this.Size = new SizeF(controlRect.Width, controlRect.Height);
        }

        /// <summary>
        /// Parent's rotation has changed, we need to update our full transform
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        void value_RotationChanged(float oldValue, float newValue)
        {
            mUpdateTransform = true;
            mUpdateFullTransform = true;
        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public GUIControl()
        {
            mControlLayout.LocationChanged += new PointDelegate(Layout_LocationChanged);
            mControlLayout.SizeChanged += new SizeDelegate(Layout_SizeChanged);
            mControlLayout.CenterChanged += new PointDelegate(Layout_CenterChanged);
            mControlLayout.RotationChanged += new FloatDelegate(Layout_RotationChanged);

            Controls.OnControlAdded += new GUIControlCollection.ControlEventHandler(Controls_OnControlAdded);
            Controls.OnControlRemoved += new GUIControlCollection.ControlEventHandler(Controls_OnControlRemoved);
        }

        /// <summary>
        /// Creates a keyframe from the control's current state
        /// </summary>
        /// <returns></returns>
        public virtual KeyFrame CreateKeyFrame()
        {
            KeyFrame frame = new KeyFrame();

            frame.Layout = (ControlLayout)this.Layout.Clone();

            if (Parent != null)
            {
                SizeF size = Parent.Size;

                float left = frame.Layout.Location.X - frame.Layout.Center.X;
                float right = left + frame.Layout.Size.Width;
                float top = frame.Layout.Location.Y - frame.Layout.Center.Y;
                float bottom = top + frame.Layout.Size.Height;

                float ratio_l = left / size.Width;
                float ratio_r = right / size.Width;
                float ratio_t = top / size.Height;
                float ratio_b = bottom / size.Height;

                frame.LeftAnchor = new AnchorData(left, ratio_l);
                frame.RightAnchor = new AnchorData(Parent.Size.Width - right, ratio_r);
                frame.TopAnchor = new AnchorData(top, ratio_t);
                frame.BottomAnchor = new AnchorData(Parent.Size.Height - bottom, ratio_b);
            }

            return frame;
        }

        /// <summary>
        /// Applies the provided keyframe to the control,
        /// overriding all needed properties
        /// </summary>
        /// <param name="frame"></param>
        public virtual void ApplyKeyFrame(KeyFrame startFrame, KeyFrame endFrame, float factor)
        {
            if (startFrame == null && endFrame == null)
                return;

            RectangleF parentRect = RectangleF.Empty;
            if (Parent != null)
            {
                parentRect = new RectangleF(0, 0, Parent.Layout.Size.Width, Parent.Layout.Size.Height);
            }

            float startLeft = startFrame.Layout.Location.X - startFrame.Layout.Center.X;
            float startRight = startLeft + startFrame.Layout.Size.Width;
            float startTop = startFrame.Layout.Location.Y - startFrame.Layout.Center.Y;
            float startBottom = startTop + startFrame.Layout.Size.Height;

            RectangleF startRect = new RectangleF(startLeft, startTop, (startRight - startLeft), (startBottom - startTop));
            PointF startCenter = startFrame.Layout.Center;

            float leftValue     = (this.AnchorFlags & AnchorFlags.LeftRelative) == 0 ? startFrame.LeftAnchor.AbsoluteValue : startFrame.LeftAnchor.RatioValue;
            float rightValue    = (this.AnchorFlags & AnchorFlags.RightRelative) == 0 ? startFrame.RightAnchor.AbsoluteValue : startFrame.RightAnchor.RatioValue;
            float topValue      = (this.AnchorFlags & AnchorFlags.TopRelative) == 0 ? startFrame.TopAnchor.AbsoluteValue : startFrame.TopAnchor.RatioValue;
            float bottomValue   = (this.AnchorFlags & AnchorFlags.BottomRelative) == 0 ? startFrame.BottomAnchor.AbsoluteValue : startFrame.BottomAnchor.RatioValue;

            Otter.Interface.Utilities.ComputeAnchoredRectangle( ref startRect, ref startCenter, ref parentRect, 
                                                                (uint)this.AnchorFlags, 
                                                                leftValue, rightValue, topValue, bottomValue);

            RectangleF endRect = RectangleF.Empty;
            PointF endCenter = PointF.Empty;

            if(endFrame != null)
            {
                float endLeft = endFrame.Layout.Location.X - endFrame.Layout.Center.X;
                float endRight = endLeft + endFrame.Layout.Size.Width;
                float endTop = endFrame.Layout.Location.Y - endFrame.Layout.Center.Y;
                float endBottom = endTop + endFrame.Layout.Size.Height;

                endRect = new RectangleF(endLeft, endTop, (endRight - endLeft), (endBottom - endTop));
                endCenter = endFrame.Layout.Center;

                leftValue   = (this.AnchorFlags & AnchorFlags.LeftRelative) == 0 ? endFrame.LeftAnchor.AbsoluteValue : endFrame.LeftAnchor.RatioValue;
                rightValue  = (this.AnchorFlags & AnchorFlags.RightRelative) == 0 ? endFrame.RightAnchor.AbsoluteValue : endFrame.RightAnchor.RatioValue;
                topValue    = (this.AnchorFlags & AnchorFlags.TopRelative) == 0 ? endFrame.TopAnchor.AbsoluteValue : endFrame.TopAnchor.RatioValue;
                bottomValue = (this.AnchorFlags & AnchorFlags.BottomRelative) == 0 ? endFrame.BottomAnchor.AbsoluteValue : endFrame.BottomAnchor.RatioValue;

                Otter.Interface.Utilities.ComputeAnchoredRectangle( ref endRect, ref endCenter, ref parentRect, 
                                                                    (uint)this.AnchorFlags,
                                                                    leftValue, rightValue, topValue, bottomValue);
            }   

            float newX = 0;
            float newY = 0;
            float newW = 0;
            float newH = 0;
            float newCX = 0;
            float newCY = 0;

            float newR = 0.0f;

            if (endFrame == null)
            {
                newCX = startCenter.X;
                newCY = startCenter.Y;

                newX = startRect.Left;
                newY = startRect.Top;

                newW = startRect.Width;
                newH = startRect.Height;

                newR = startFrame.Layout.Rotation;
            }
            else
            {
                newCX = startCenter.X * (1.0f - factor) + endCenter.X * factor;
                newCY = startCenter.Y * (1.0f - factor) + endCenter.Y * factor;

                newX = startRect.Left * (1.0f - factor) + endRect.Left * factor;
                newY = startRect.Top * (1.0f - factor) + endRect.Top * factor;

                newW = (startRect.Width) * (1.0f - factor) + (endRect.Width) * factor;
                newH = (startRect.Height) * (1.0f - factor) + (endRect.Height) * factor;

                newR = startFrame.Layout.Rotation * (1.0f - factor) + endFrame.Layout.Rotation * factor;
            }

            Center = new PointF(newCX, newCY);
            Location = new PointF(newX + newCX, newY + newCY);
            Size = new SizeF(newW, newH);
            Rotation = newR;
        }

        /// <summary>
        /// Draws the control
        /// </summary>
        public virtual void Draw(Otter.Interface.Graphics graphics)
        {
            foreach (GUIControl control in Controls)
            {
                if (control.Hidden && control.GetType() != typeof(GUIMask))
                    continue;

                graphics.PushMatrix(control.Transform.Entries);

                //check whether we have a mask to render first
                int inheritedMaskID = control.InheritedMask;
                if (inheritedMaskID != -1)
                {
                    GUIMask maskControl = ParentView.Controls.GetControl(inheritedMaskID) as GUIMask;
                    maskControl.DrawMask(Otter.Interface.Graphics.Instance);
                }

                control.Draw(graphics);
                graphics.PopMatrix();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bw"></param>
        public virtual void Export(PlatformBinaryWriter bw)
        {
            bw.Write(ID);
            byte[] bytes = Utils.StringToBytes(mName, 64);
            bytes[63] = 0;
            bw.Write(bytes, 0, 64);

            bw.Write((float)this.Layout.Location.X);
            bw.Write((float)this.Layout.Location.Y);
            bw.Write((float)this.Layout.Center.X);
            bw.Write((float)this.Layout.Center.Y);
            bw.Write((float)this.Layout.Size.Width);
            bw.Write((float)this.Layout.Size.Height);
            bw.Write((float)this.Layout.Rotation);

            bw.Write((uint)this.AnchorFlags);

            bw.Write(this.InheritedMask);
        }

        public virtual void ExportControls(PlatformBinaryWriter bw)
        {
            bw.Write(Controls.Count);
            foreach (GUIControl control in Controls)
            {
                control.Export(bw);
            }
        }

        /// <summary>
        /// Performs a hittest on this control.  Returns itself if hit.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public virtual GUIControl HitTest(PointF location)
        {
            if (Locked || Hidden)
                return null;

            // Since the list is from back to front,
            // cycle in reverse.
            int cnt = Controls.Count;
            for (int i = cnt - 1; i >= 0; i--)
            {
                GUIControl control = Controls[i].HitTest(location);
                if (control != null)
                    return control;
            }

            Matrix mtx = FullTransform;
            Matrix invert = Matrix.Invert(mtx);

            Vector4 v = new Vector4(location.X, location.Y, 0.0f, 1.0f);
            v = Vector4.Transform(v, invert);

            if (v.X >= 0.0f && v.X <= Layout.Size.Width && v.Y >= 0.0 && v.Y <= Layout.Size.Height)
                return this;

            return null;
        }

        /// <summary>
        /// Gets the area that a point lies in.  Location
        /// is relative to the control itself.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public virtual PointArea GetPointArea(PointF location)
        {
            float halfWidth = Layout.Size.Width / 2.0f;
            float halfHeight = Layout.Size.Height / 2.0f;
            
            PointArea pointArea = PointArea.None;

            /*
            if (location.X >= (Center.X - 4) && location.X <= (Center.X + 4) &&
               location.Y >= (Center.Y - 4) && location.Y <= (Center.Y + 4))
            {
                pointArea |= PointArea.Center;
            }
             */

            if (location.X >= 0 && location.X <= Layout.Size.Width && location.Y >= 0 && location.Y <= Layout.Size.Height)
            {
                pointArea |= PointArea.Body;
            }

            {
                bool xl = (location.X >= -8 && location.X <= 0);
                bool xm = (location.X >= (halfWidth - 4) && location.X < (halfWidth + 4));
                bool xr = (location.X >= (Layout.Size.Width) && location.X <= (Layout.Size.Width + 8));
                bool yt = (location.Y >= -8 && location.Y <= 0);
                bool ym = (location.Y >= (halfHeight - 4) && location.Y < (halfHeight + 4));
                bool yb = (location.Y >= (Layout.Size.Height) && location.Y <= (Layout.Size.Height + 8));

                // Left Edge
                if (xl)
                {
                    if (yt)
                        pointArea |= PointArea.Top | PointArea.Left;
                    else if (ym)
                        pointArea |= PointArea.Left;
                    else if (yb)
                        pointArea |= PointArea.Bottom | PointArea.Left;
                }
                // Middle
                else if (xm)
                {
                    if (yt)
                        pointArea |= PointArea.Top;
                    else if (yb)
                        pointArea |= PointArea.Bottom;
                }
                // Right Edge
                else if (xr)
                {
                    if (yt)
                        pointArea |= PointArea.Top | PointArea.Right;
                    else if (ym)
                        pointArea |= PointArea.Right;
                    else if (yb)
                        pointArea |= PointArea.Bottom | PointArea.Right;
                }
            }

            return pointArea;
        }

        /// <summary>
        /// Updates the local transform if necessary
        /// </summary>
        private void UpdateTransform()
        {
            if (mUpdateTransform)
            {
                Matrix offsetTrans = Matrix.Translation(-mControlLayout.Center.X, -mControlLayout.Center.Y, 0.0f);
                Matrix rotation = Matrix.RotationZ((mControlLayout.Rotation / 180.0f) * (float)Math.PI);
                Matrix locTrans = Matrix.Translation(mControlLayout.Location.X, mControlLayout.Location.Y, 0.0f);

                // Matrix mul order is left to right, ie A * B is A followed by B.
                mTransform = (offsetTrans * rotation) * locTrans;
                mTransformInv = Matrix.Invert(mTransform);
                mUpdateTransform = false;
            }
        }

        /// <summary>
        /// Updates the full transform if necessary
        /// </summary>
        private void UpdateFullTransform()
        {
            if (mUpdateFullTransform)
            {
                Matrix local = Transform;
                Matrix parent = (Parent != null) ? Parent.FullTransform : Matrix.Identity;

                mFullTransform = local * parent;
                mFullTransformInv = Matrix.Invert(mFullTransform);
                mUpdateFullTransform = false;
            }
        }

        /// <summary>
        /// Abstract clone function intended for derived objects to implement.
        /// </summary>
        /// <returns>
        /// Deep copy of this object.
        /// </returns>
        public abstract object Clone();

        /// <summary>
        /// Disposes of the object.  Destroys the textures that
        /// were allocated in the scene.
        /// </summary>
        public virtual void Dispose()
        {
            foreach (int textureID in TextureIDs)
                Scene.DestroyTexture(textureID);
        }

        /// <summary>
        /// ToString Override
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
