using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

using WeifenLuo.WinFormsUI.Docking;

using Otter.Project;
using Otter.Interface;
using Otter.Editor.Forms;
using Otter.Commands;
using Otter.Editor.Commands.ControlCommands;
using Otter.Export;
using Otter.UI;
using Otter.Containers;

namespace Otter.Editor.ContentViews
{
    public partial class SceneView : DockContent
    {
        #region Enumerations
        [Flags]
        private enum Constraint
        {
            None    = 0,
            X       = 1 << 1,
            Y       = 1 << 2,
            Scale   = 1 << 3
        };
        #endregion

        #region Event and Delegates
        public delegate void ControlEventHandler(object sender, GUIControl control);
        public delegate void ControlsEventHandler(object sender, List<GUIControl> controls);
        public delegate void ViewEventHandler(object sender, GUIView view);

        public event ControlEventHandler GUIControlUpdated = null;
        public event ControlsEventHandler SelectedControlsChanged = null;
        public event ViewEventHandler ActiveViewChanged = null;
        #endregion

        #region Data
        private const int mNudgeSpeedWithShift = 10;

        private CommandManager mCommandManager = null;
        private GUIScene mScene = null;
        private GUIView mActiveView = null;
        private NotifyingList<GUIControl> mSelectedControls = new NotifyingList<GUIControl>();

        private PointArea mPointArea = PointArea.None;
        private PointF mLocationOffset = PointF.Empty;

        private Type mCreateControlType = null;
        private AddControlCommand mAddControlCommand = null;

        private List<Control> mViewButtons = new List<Control>();

        private PointF mCameraLocation = PointF.Empty;
        private PointF mMouseMoveStartPos = PointF.Empty;
        private PointF mLastMousePos = PointF.Empty;

        private PointF mControlStartPos = PointF.Empty;
        private PointF mControlStartCenter = PointF.Empty;
        private SizeF mControlStartSize = SizeF.Empty;

        private int mBackgroundTextureID = -1;
        private float mZoom = 1.0f;

        private Color mScreenBoundsColor = Color.White;
        private Platform mShowPlatformSafety = null;
        private bool mShowGrid = false;
        private bool mEnableSnapping = false;

        private int mCommandMark = 0;
        #endregion

        #region Properties
        /// <summary>
        /// Retrieves the Scene ParentView's command manager
        /// </summary>
        public CommandManager CommandManager
        {
            get { return mCommandManager; }
        }

        /// <summary>
        /// Returns the GUI Scene that this sceneView is managing.
        /// </summary>
        public GUIScene Scene
        {
            get { return mScene; }
        }
         
        /// <summary>
        /// Gets / Sets the active sceneView
        /// </summary>
        public GUIView ActiveView
        {
            get 
            { 
                return mActiveView; 
            }
            set
            {
                if (mActiveView != value)
                {
                    mActiveView = value;
                    mSelectedControls.Clear();

                    NotifyActiveViewChanged(mActiveView);
                    RefreshRenderPanel();
                }
            }
        }

        /// <summary>
        /// Gets / Sets the primary selected control
        /// </summary>
        public GUIControl PrimaryControl
        {
            get
            { 
                return (mSelectedControls.Count >= 1) ? mSelectedControls[0] : null; 
            }
            set
            {
                if (value is GUIView)
                    return;

                GUIControl old = PrimaryControl;
                if(old != value)
                {
                    if (mSelectedControls.Contains(value))
                    {
                        mSelectedControls.Remove(value);
                    }
                    else
                    {
                        mSelectedControls.Clear();
                    }

                    if(value != null)
                        mSelectedControls.Insert(0, value);

                    RefreshRenderPanel();
                }
            }
        }

        /// <summary>
        /// Gets / Sets the selected control
        /// </summary>
        public NotifyingList<GUIControl> SelectedControls
        {
            get 
            { 
                return mSelectedControls; 
            }
            set
            {
                NotifyingList<GUIControl> list = value;
                if (list == null)
                    list = new NotifyingList<GUIControl>();

                List<GUIControl> intersection = new List<GUIControl>(mSelectedControls.Intersect(list));
                if (mSelectedControls.Count == list.Count && intersection.Count == list.Count)
                    return;

                mSelectedControls.SuppressEvents = true;

                mSelectedControls.Clear();
                if (list != null)
                    mSelectedControls.AddRange(list.Where(a => !(a is GUIView)));

                mSelectedControls.SuppressEvents = false;

                NotifyControlSelectionChanged(SelectedControls);

                RefreshRenderPanel();
            }
        }

        /// <summary>
        /// Gets / Sets the current zoom level
        /// </summary>
        public float Zoom
        {
            get { return mZoom; }
            set 
            {
                mZoom = value;

                if (mZoom < 0.05f)
                    mZoom = 0.05f;
                if (mZoom >= 3.0f)
                    mZoom = 3.0f;

                mZoom = (float)Math.Round(mZoom, 2);
                    
                UpdateProjectionMatrix();
                RefreshRenderPanel();

                mZoomTextBox.Text = String.Format("{0}%", mZoom * 100);
            }
        }

        /// <summary>
        /// Gets / Sets the control type to create on the next mouse-down.
        /// </summary>
        public Type CreateControlType
        {
            get { return mCreateControlType; }
            set { mCreateControlType = value; }
        }

        /// <summary>
        /// Gets / Sets whether or not the scene has been modified
        /// </summary>
        public bool Modified
        {
            get
            {
                return (mCommandManager.CurrentCommandIndex != mCommandMark);
            }
            set
            {
                if (value == true)
                {
                    mCommandMark = -1;
                }
                else
                {
                    mCommandMark = mCommandManager.CurrentCommandIndex;
                }
            }
        }

        public bool PromptSave { get; set; }
        #endregion
         
        #region Event Notifiers
        /// <summary>
        /// Notifies that the control selection has changed
        /// </summary>
        /// <param name="control"></param>
        private void NotifyControlSelectionChanged(List<GUIControl> controls)
        {
            if (SelectedControlsChanged != null)
                SelectedControlsChanged(this, controls);
        }

        /// <summary>
        /// Notifies that the active sceneView has changed
        /// </summary>
        /// <param name="control"></param>
        private void NotifyActiveViewChanged(GUIView view)
        {
            if (ActiveViewChanged != null)
                ActiveViewChanged(this, view);
        }

        /// <summary>
        /// Called when the control's been moved in some
        /// </summary>
        /// <param name="control"></param>
        private void NotifyControlUpdated(GUIControl control)
        {
            if (GUIControlUpdated != null)
                GUIControlUpdated(this, control);
        }
        #endregion

        #region Event Handlers : Context Menu Strip
        /// <summary>
        /// Context Menu Strip is about to be opened.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            mCreateKeyFrameToolStripMenuItem.Enabled = (PrimaryControl != null);
            mDeleteStripMenuItem.Enabled = (PrimaryControl != null);
            mBringToFrontToolStripMenuItem.Enabled = (PrimaryControl != null) && (ActiveView.Controls.IndexOf(PrimaryControl) != (ActiveView.Controls.Count - 1));
            mSendToBackToolStripMenuItem.Enabled = (PrimaryControl != null) && ActiveView.Controls.IndexOf(PrimaryControl) != 0;

            mCustomActionsToolStripMenuItem.DropDownItems.Clear();

            List<Type> addedTypes = new List<Type>();
            foreach (GUIControl control in SelectedControls)
            {
                Type type = control.GetType();

                // Make sure we don't add the same type multiple times
                if (addedTypes.Contains(type))
                    continue;

                addedTypes.Add(type);
                ToolStripMenuItem item = new ToolStripMenuItem(type.ToString().Replace("Otter.UI.GUI", ""));

                MethodInfo[] methodInfos = type.GetMethods();
                foreach (MethodInfo info in methodInfos)
                {
                    // Find the Method Attribute
                    System.Attribute[] attributes = (System.Attribute[])info.GetCustomAttributes(typeof(Otter.UI.Attributes.MethodAttribute), true);
                    if (attributes != null && attributes.Length > 0)
                    {
                        Otter.UI.Attributes.MethodAttribute methodAttr = (Otter.UI.Attributes.MethodAttribute)attributes[0];

                        if (!mCustomActionsToolStripMenuItem.DropDownItems.Contains(item))
                            mCustomActionsToolStripMenuItem.DropDownItems.Add(item);

                        item.Tag = type;

                        ToolStripMenuItem methodItem = new ToolStripMenuItem(methodAttr.ToString());
                        methodItem.Tag = info;
                        methodItem.Click += new EventHandler(MethodItem_Click);

                        item.DropDownItems.Add(methodItem);
                    }
                }

                mCustomActionsToolStripMenuItem.Enabled = mCustomActionsToolStripMenuItem.DropDownItems.Count > 0;
            }

            if (mCustomActionsToolStripMenuItem.DropDownItems.Count == 1)
            {
                ToolStripMenuItem item = (ToolStripMenuItem)mCustomActionsToolStripMenuItem.DropDownItems[0];

                mCustomActionsToolStripMenuItem.DropDownItems.Clear();
                mCustomActionsToolStripMenuItem.DropDownItems.AddRange(item.DropDownItems);
            }
        }

        /// <summary>
        /// Called when the user clicked on a method item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MethodItem_Click(object sender, EventArgs e)
        {
            MethodInfo methodInfo = ((ToolStripMenuItem)sender).Tag as MethodInfo;
            if (methodInfo == null)
                return;

            foreach (GUIControl control in SelectedControls)
            {
                if (control.GetType() == methodInfo.DeclaringType)
                {
                    ControlLayout beforeLayout = (ControlLayout)control.Layout.Clone();
                    methodInfo.Invoke(control, null);
                    ControlLayout afterLayout = (ControlLayout)control.Layout.Clone();

                    if (!afterLayout.Equals(beforeLayout) && Otter.Editor.Properties.Settings.Default.AutoKeyFrame && control.ParentView != null)
                        control.ParentView.CreateKeyFrame(control);
                }
            }

            RefreshRenderPanel();
        } 

        /// <summary>
        /// User clicked on the "Bring To Front" button.  Bring the currently
        /// selected controls to the front.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mBringToFrontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedControls.Count == 0)
                return;

            mCommandManager.AddCommand(new ChangeOrderCommand(SelectedControls, -1), true);
        }

        /// <summary>
        /// User clicked ont he "Send To Back" button.  Send the currently selected
        /// controls to the back.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mSendToBackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (PrimaryControl == null)
                return;

            mCommandManager.AddCommand(new ChangeOrderCommand(SelectedControls, 0), true);
        }

        private void UpdateIDs(GUIControl control)
        {
            control.ID = control.ParentView.NextControlID++;
            foreach (GUIControl child in control.Controls)
                UpdateIDs(child);
        }

        /// <summary>
        /// User selected to duplicate the selected control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mDuplicateStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedControls.Count  == 0)
                return;

            List<GUIControl> newControls = new List<GUIControl>();

            foreach (GUIControl control in SelectedControls)
            {
                GUIControl copy = (GUIControl)control.Clone();
                copy.Name = "Copy of " + copy.Name;

                // Need to change the control's IDs recursively
                UpdateIDs(copy);

                mCommandManager.AddCommand(new AddControlCommand(ActiveView, control.Parent, copy), true);

                newControls.Add(copy);
            }

            SelectedControls.Clear();
            SelectedControls.AddRange(newControls);

            RefreshRenderPanel();
        }

        /// <summary>
        /// User selected to delete the selected control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mDeleteStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (PrimaryControl == null)
                return;

            if (MessageBox.Show("Are you sure you want to delete this control?", "Warning", MessageBoxButtons.YesNo) == DialogResult.No)
                return;

            DeleteControls(SelectedControls);
            SelectedControls.Clear();
            RefreshRenderPanel();
        }

        /// <summary>
        /// User has selected to create a keyframe for the selected control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mCreateKeyFrameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (PrimaryControl == null)
                return;

            foreach (GUIControl control in SelectedControls)
                ActiveView.CreateKeyFrame(control);
        }
        #endregion

        #region Event Handlers : Render Panel
        /// <summary>
        /// Called when the render panel has loaded 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mRenderPanel_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Called when the Render Panel has been disposed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mRenderPanel_Disposed(object sender, System.EventArgs e)
        {
        }

        /// <summary>
        /// Paints the render panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mRenderPanel_Paint(object sender, PaintEventArgs e)
        {
            if (this.DesignMode)
                return;

            UpdateViewMatrix();
            Otter.Interface.Graphics.Instance.SetBounds(mScene.Resolution.Width, mScene.Resolution.Height, mScreenBoundsColor.ToArgb());
            Otter.Interface.Graphics.Instance.Begin((int)mRenderPanel.RenderContext);

            try
            {
                if (ActiveView != null)
                {
                    DrawBackground();

                    foreach (GUIControl control in ActiveView.Controls)
                    {
                        if (control.Hidden && control.GetType() != typeof(GUIMask))
                            continue;

                        Otter.Interface.Graphics.Instance.PushMatrix(control.Transform.Entries);

                        //check whether we have a mask to render first
                        int inheritedMaskID = control.InheritedMask;
                        if (inheritedMaskID != -1)
                        {
                            GUIMask maskControl = ActiveView.Controls.GetControl(inheritedMaskID) as GUIMask;
                            maskControl.DrawMask(Otter.Interface.Graphics.Instance);
                        }

                        control.Draw(Otter.Interface.Graphics.Instance);
                        Otter.Interface.Graphics.Instance.PopMatrix();
                    }
                }

                if (mShowGrid)
                {
                    int w = mRenderPanel.Width;
                    int h = mRenderPanel.Height;
                    int inc = Otter.Editor.Properties.Settings.Default.ViewGridIncrement;
                    int col = Otter.Editor.Properties.Settings.Default.ViewBoundsColor.ToArgb();

                    if (inc > 0)
                    {
                        PointF tl = ClientToViewport(new Point(0, 0));
                        PointF br = ClientToViewport(new Point(w, h));

                        for (float x = (((int)tl.X) / inc) * inc; x <= ((int)(br.X) + inc); x += inc)
                            Otter.Interface.Graphics.Instance.DrawLine(x, tl.Y, 0, x, br.Y, 0, col);

                        for (float y = (((int)tl.Y) / inc) * inc; y <= ((int)(br.Y) + inc); y += inc)
                            Otter.Interface.Graphics.Instance.DrawLine(tl.X, y, 0, br.X, y, 0, col);
                    }
                }

                if (mEnableSnapping && PrimaryControl != null)
                {
                    DrawSnapTargets(ActiveView.Controls);
                }

                foreach (GUIControl selectedControl in SelectedControls)
                {
                    if (selectedControl != PrimaryControl)
                        DrawHighlightBox(selectedControl,
                            false,
                            Otter.Editor.Properties.Settings.Default.ViewSelectColor,
                            Otter.Editor.Properties.Settings.Default.ViewInnerSelectColor);
                }

                if (PrimaryControl != null)
                {
                    DrawHighlightBox(PrimaryControl,
                        SelectedControls.Count == 1,
                        (SelectedControls.Count == 1) ? Otter.Editor.Properties.Settings.Default.ViewSelectColor : Color.Red,
                        Otter.Editor.Properties.Settings.Default.ViewInnerSelectColor);
                }

                if (mShowPlatformSafety != null)
                {
                    int col = Color.Red.ToArgb();
                    float w = mScene.Resolution.Width;
                    float h = mScene.Resolution.Height;
                    float sw = w * mShowPlatformSafety.SafeDisplayWidth;
                    float sh = h * mShowPlatformSafety.SafeDisplayHeight;
                    float x = (w / 2.0f) - (sw / 2.0f);
                    float y = (h / 2.0f) - (sh / 2.0f);

                    // Draw the actual screen bounds
                    Otter.Interface.Graphics.Instance.DrawLine(0, 0, 0, w, 0, 0, col);
                    Otter.Interface.Graphics.Instance.DrawLine(0, h, 0, w, h, 0, col);
                    Otter.Interface.Graphics.Instance.DrawLine(0, 0, 0, 0, h, 0, col);
                    Otter.Interface.Graphics.Instance.DrawLine(w, 0, 0, w, h, 0, col);

                    // Draw the safe display bounds
                    Otter.Interface.Graphics.Instance.DrawLine(x + 0, y + 0, 0, x + sw, y + 0, 0, col);
                    Otter.Interface.Graphics.Instance.DrawLine(x + 0, y + sh, 0, x + sw, y + sh, 0, col);
                    Otter.Interface.Graphics.Instance.DrawLine(x + 0, y + 0, 0, x + 0, y + sh, 0, col);
                    Otter.Interface.Graphics.Instance.DrawLine(x + sw, y + 0, 0, x + sw, y + sh, 0, col);
                }
            }
            catch (Exception ex)
            {
                System.Console.Write("Exception: " + ex);
            }

            Otter.Interface.Graphics.Instance.End();

            if (Otter.Interface.Graphics.Instance.WasDeviceLost())
                RefreshRenderPanel();
        }

        private void DrawSnapTargets(GUIControlCollection controls)
        {
            int col = Otter.Editor.Properties.Settings.Default.ViewBoundsColor.ToArgb();
            foreach (GUIControl control in controls)
            {
                if (SelectedControls.Contains(control))
                    continue;

                DrawSnapTargets(control.Controls);
                DrawHighlightBox(control, false, Otter.Editor.Properties.Settings.Default.ViewBoundsColor, Otter.Editor.Properties.Settings.Default.ViewBoundsColor);                
            }
        }

        /// <summary>
        /// Called when the render panel has been resized.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mRenderPanel_Resize(object sender, EventArgs e)
        {
            if (this.DesignMode)
                return;

            UpdateProjectionMatrix();
            RefreshRenderPanel();
        }

        /// <summary>
        /// Retrieves a texture either by path or by resource.  If the texture
        /// cannot be found by path, it is created from the resource name and saved
        /// at the path.
        /// </summary>
        /// <param name="texturePath"></param>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        private int GetTexture(string texturePath, string resourceName)
        {
            UI.Resources.TextureInfo info = mScene.GetTextureInfo(texturePath);
            if (info != null)
            {
                info.Load();
                if (info != null && info.TextureID != -1)
                    return info.ID;
            }

            string fullPath = GUIProject.CurrentProject.ProjectDirectory + "/" + texturePath;
            if (!System.IO.File.Exists(fullPath))
            {
                Stream stream = null;

                Assembly assembly = Assembly.GetExecutingAssembly();
                stream = assembly.GetManifestResourceStream(resourceName);
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, (int)stream.Length);
                stream.Close();

                if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(fullPath)))
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(fullPath));

                FileStream fs = new FileStream(fullPath, FileMode.Create);
                fs.Write(bytes, 0, bytes.Length);
                fs.Close();
            }

            return mScene.CreateTexture(texturePath);
        }

        /// <summary>
        /// User clicked on the render panel.  See if we selected any controls.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mRenderPanel_MouseDown(object sender, MouseEventArgs e)
        {
            PointF hitLocation = ClientToViewport(e.Location);
            mLastMousePos = hitLocation;
            mMouseMoveStartPos = hitLocation;

            if (ActiveView == null)
                return;

            if (e.Button == MouseButtons.Left)
            {
                mPointArea = PointArea.None;

                if (CreateControlType != null)
                {
                    System.Reflection.ConstructorInfo constructorInfo = CreateControlType.GetConstructor(System.Type.EmptyTypes);
                    GUIControl control = constructorInfo.Invoke(null) as GUIControl;
                    control.AnchorFlags = Otter.Editor.Properties.Settings.Default.AnchorFlags;

                    if (control != null)
                    {
                        control.Location = new PointF(hitLocation.X, hitLocation.Y);
                        control.ID = mActiveView.NextControlID++;
                        control.Scene = ActiveView.Scene;
                        control.Name = GetUniqueControlName(control);

                        // Label Specific
                        if (control is GUILabel && GUIProject.CurrentProject.Fonts.Count > 0)
                        {
                            ((GUILabel)control).FontID = GUIProject.CurrentProject.Fonts[0].ID;
                        }

                        // Slider Specific
                        if (control is GUISlider)
                        {
                            ((GUISlider)control).StartTexture = GetTexture("Textures/Slider_Start.png", "Otter.Editor.res.ControlTextures.Slider_Start.png");
                            ((GUISlider)control).MiddleTexture = GetTexture("Textures/Slider_Middle.png", "Otter.Editor.res.ControlTextures.Slider_Middle.png");
                            ((GUISlider)control).EndTexture = GetTexture("Textures/Slider_End.png", "Otter.Editor.res.ControlTextures.Slider_End.png"); ;
                            ((GUISlider)control).ThumbTexture = GetTexture("Textures/Slider_Thumb.png", "Otter.Editor.res.ControlTextures.Slider_Thumb.png"); ;
                        }

                        // Toggle Specific
                        if (control is GUIToggle)
                        {
                            ((GUIToggle)control).OnTexture = GetTexture("Textures/Toggle_On.png", "Otter.Editor.res.ControlTextures.Toggle_On.png");
                            ((GUIToggle)control).OffTexture = GetTexture("Textures/Toggle_Off.png", "Otter.Editor.res.ControlTextures.Toggle_Off.png");
                        }

                        mAddControlCommand = new AddControlCommand(ActiveView, null, control);
                        mCommandManager.AddCommand(mAddControlCommand, true);

                        SelectedControls.Clear();
                        SelectedControls.Add(control);
                        mPointArea = PointArea.Bottom | PointArea.Right;
                    }
    
                    CreateControlType = null;
                }
                // Hit test the selected controls
                else
                {
                    // Check to see if we've hit any of our selected controls in the body (only).
                    // If so, move the control to the front of the list and apply the point area accordingly.
                    foreach (GUIControl control in SelectedControls)
                    {
                        PointF localPoint = ViewportToControl(hitLocation, control);
                        mPointArea = control.GetPointArea(localPoint);

                        if (mPointArea == PointArea.Body)
                        {
                            PrimaryControl = control;

                            Vector4 absPos = Vector4.Transform(new Vector4(control.Center.X, control.Center.Y, 0.0f, 1.0f), control.FullTransform);
                            mLocationOffset = new PointF(hitLocation.X - absPos.X, hitLocation.Y - absPos.Y);
                            
                            break;
                        }
                    }

                    if (SelectedControls.Count > 1 && mPointArea != PointArea.Body)
                        mPointArea = PointArea.None;
                }

                // If we clicked outside of the control select a new one if possible.
                // Uncomment the PointArea.Body to select a new control if the body was hit
                if (mPointArea == PointArea.None) // || mPointArea == PointArea.Body)
                {
                    GUIControl control = ActiveView.HitTest(hitLocation);
                    control = (control is GUIView) ? null : control;    // Make sure we don't select the view

                    if (!(control is GUIView) && !SelectedControls.Contains(control))
                    {
                        if (Control.ModifierKeys != Keys.Control)
                            SelectedControls.Clear();

                        if(control != null)
                            SelectedControls.Add(control);

                        PrimaryControl = control;

                        mPointArea = PointArea.None;

                        // TODO : The below if-else is a copy-paste from above.  FIX IT.
                        foreach (GUIControl selectedControl in SelectedControls)
                        {
                            PointF localPoint = ViewportToControl(hitLocation, selectedControl);
                            mPointArea = selectedControl.GetPointArea(localPoint);

                            if (mPointArea == PointArea.Body)
                            {
                                Vector4 absPos = Vector4.Transform(new Vector4(control.Center.X, control.Center.Y, 0.0f, 1.0f), PrimaryControl.FullTransform);
                                mLocationOffset = new PointF(hitLocation.X - absPos.X, hitLocation.Y - absPos.Y);
                            
                                break;
                            }
                        }

                        if (SelectedControls.Count > 1 && mPointArea != PointArea.Body)
                            mPointArea = PointArea.None;
                    }
                }

                if (PrimaryControl != null)
                {
                    mControlStartPos = PrimaryControl.Location;
                    mControlStartCenter = PrimaryControl.Center;
                    mControlStartSize = PrimaryControl.Size;
                }

                RefreshRenderPanel();
            }
        }

        /// <summary>
        /// Called when a mouse button has been released
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mRenderPanel_MouseUp(object sender, MouseEventArgs e)
        {
            mPointArea = PointArea.None;
            if (e.Button == MouseButtons.Left)
            {
                // We just finished placing a control - override frame 0 with the 
                // control's new layout, without interfering with the command manager's undo/redo
                if (mAddControlCommand != null)
                {
                    Otter.UI.Animation.GUIAnimation onActivate = mAddControlCommand.Controls[0].ParentView.Animations["OnActivate"];
                    if (onActivate != null)
                        onActivate.CreateKeyFrame(mAddControlCommand.Controls[0], 0);
                    mAddControlCommand = null;
                }
                else if (PrimaryControl != null)
                {
                    if (mControlStartPos != PrimaryControl.Location)
                        mCommandManager.AddCommand(new MoveControlCommand(SelectedControls, new PointF(PrimaryControl.Location.X - mControlStartPos.X, PrimaryControl.Location.Y - mControlStartPos.Y)), false);

                    if (mControlStartSize != PrimaryControl.Size || mControlStartCenter != PrimaryControl.Center)
                    {
                        mCommandManager.AddCommand(new ResizeControlCommand(PrimaryControl, mControlStartCenter, mControlStartSize, PrimaryControl.Center, PrimaryControl.Size), false);
                    }
                }
            }
        }

        /// <summary>
        /// Handles the mouse-move event on the Render Panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mRenderPanel_MouseMove(object sender, MouseEventArgs e)
        {
            PointF mousePos = ClientToViewport(e.Location);

            float dx = (float)Math.Truncate((double)(mLastMousePos.X - mousePos.X));
            float dy = (float)Math.Truncate((double)(mLastMousePos.Y - mousePos.Y));

            if (dx == 0.0f && dy == 0.0f)
                return;

            if (e.Button == MouseButtons.None)
            {
                // Detect if we're going to hover over any grips.
                if (PrimaryControl != null)
                {
                    PointF pt = ViewportToControl(mousePos, PrimaryControl);
                    PointArea pointArea = PrimaryControl.GetPointArea(pt);

                    if (pointArea != PointArea.None && pointArea != PointArea.Body)
                        Cursor.Current = Cursors.Hand;
                    else
                        Cursor.Current = Cursors.Default;
                }
            }
            // Middle-mouse drags the sceneView around
            else if (e.Button == MouseButtons.Middle)
            {
                if (dx != 0.0f || dy != 0.0f)
                {
                    mCameraLocation.X += dx;
                    mCameraLocation.Y += dy;

                    UpdateViewMatrix();
                    RefreshRenderPanel();
                }
            }
            else if (e.Button == MouseButtons.Left)
            {
                // Cannot move the control is if it's locked
                if (PrimaryControl != null && !PrimaryControl.Locked)
                {
                    Constraint constraint = Constraint.None;
                    if((Control.ModifierKeys & Keys.Shift) != 0)
                    {
                        if ((mPointArea & (PointArea.Left | PointArea.Top)) == (PointArea.Left | PointArea.Top) ||
                            (mPointArea & (PointArea.Left | PointArea.Bottom)) == (PointArea.Left | PointArea.Bottom) ||
                            (mPointArea & (PointArea.Right | PointArea.Top)) == (PointArea.Right | PointArea.Top) ||
                            (mPointArea & (PointArea.Right | PointArea.Bottom)) == (PointArea.Right | PointArea.Bottom))
                        {
                            constraint |= Constraint.Scale;
                        }

                        if(mPointArea == PointArea.Body)
                        {
                            if(Math.Abs(mousePos.X - mMouseMoveStartPos.X) > Math.Abs(mousePos.Y - mMouseMoveStartPos.Y))
                                constraint |= Constraint.X;
                            else
                                constraint |= Constraint.Y;
                        }
                    }

                    HandleMouseDrag(ref mLastMousePos, ref mousePos, ref mLocationOffset, constraint);
                }
            }

            mLastMousePos = ClientToViewport(e.Location);
        }
        #endregion

        #region Event Handlers : Render Panel Keys

        /// <summary>
        /// Preview key event - used to trigger handling of arrow keys.
        /// </summary>
        private void mRenderPanel_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Up ||
                e.KeyCode == Keys.Down ||
                e.KeyCode == Keys.Left ||
                e.KeyCode == Keys.Right)
            {
                e.IsInputKey = true;
            }            
        }

        /// <summary>
        /// User hit a button while the render panel had focus.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mRenderPanel_KeyDown(object sender, KeyEventArgs e)
        {
            // Limit functionality if we're placing a control
            if (mAddControlCommand != null)
                return;

            if (e.KeyCode == Keys.Escape)
            {
                SelectedControls.Clear();
                RefreshRenderPanel();
            }
            else if (e.KeyCode == Keys.Delete)
            {
                if (PrimaryControl == null)
                    return;

                if (MessageBox.Show("Are you sure you want to delete these control(s)?", "Warning", MessageBoxButtons.YesNo) == DialogResult.No)
                    return;

                DeleteControls(SelectedControls);

                SelectedControls.Clear();
                RefreshRenderPanel();
            }
            else if (e.KeyCode == Keys.G)
            {
                mShowGridToolStripButton_Click(sender, e);
            }
            else if (e.KeyCode == Keys.X)
            {
                mEnableSnapping = true;
                RefreshRenderPanel();
            }
            else if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
            {
                float xOffset = 0;
                float yOffset = 0;

                switch (e.KeyCode)
                {
                    case Keys.Up: yOffset = e.Shift ? -mNudgeSpeedWithShift : -1; break;
                    case Keys.Down: yOffset = e.Shift ? mNudgeSpeedWithShift : 1; break;
                    case Keys.Left: xOffset = e.Shift ? -mNudgeSpeedWithShift : -1; break;
                    case Keys.Right: xOffset = e.Shift ? mNudgeSpeedWithShift : 1; break;
                }
                
                IEnumerable<GUIControl> controls = SelectedControls.Where<GUIControl>(a => !a.Locked);

                if (controls.Count() > 0 && (xOffset != 0 || yOffset != 0))
                {
                    MoveControlCommand cmd = new MoveControlCommand(new List<GUIControl>(controls), new PointF(xOffset, yOffset));
                    this.CommandManager.AddCommand(cmd, true);

                    RefreshRenderPanel();
                    Globals.PropertiesView.RefreshProperties();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mRenderPanel_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.X)
            {
                mEnableSnapping = false;
                RefreshRenderPanel();
            }
        }
        #endregion

        #region Event Handlers : Scene Toolstrip
        /// <summary>
        /// Called when the resolution combo box has changed.  Update the resolution for this scene.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mResolutionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            mScene.Resolution = mResolutionComboBox.SelectedItem as Resolution;
            RefreshRenderPanel();
        }

        /// <summary>
        /// Called when the user pressed a key in the text box.  Listen for "enter"
        /// and then parse the text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mZoomTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                float zoom = Zoom;
                try
                {
                    zoom = float.Parse(mZoomTextBox.Text.Replace('%', '\0')) / 100.0f;
                }
                catch (Exception) 
                {
                }

                Zoom = zoom;
            }
        }
        #endregion

        #region Event Handlers : Command Manager
        /// <summary>
        /// Called whenever the command manager has added a command
        /// </summary>
        /// <param name="command"></param>
        void mCommandManager_OnCommandAdded(Command command)
        {
            // If the command manager executed a command BEFORE the last
            // mark, we forcibly flag the scene as modified
            if (mCommandMark > mCommandManager.CurrentCommandIndex && mCommandMark >= mCommandManager.Commands.Count)
                Modified = true;

            if (command is ControlCommand)
            {
                ControlCommand controlCommand = command as ControlCommand;
                if (Otter.Editor.Properties.Settings.Default.AutoKeyFrame)
                {
                    foreach (GUIControl control in controlCommand.Controls)
                    {
                        if (control.ParentView != null)
                            control.ParentView.CreateKeyFrame(control);
                    }
                }
            }

            RefreshRenderPanel();
        }

        /// <summary>
        /// Called whenever the command manager has executed a command
        /// </summary>
        /// <param name="command"></param>
        void mCommandManager_OnExecute(Command command)
        {
            // If the command manager executed a command BEFORE the last
            // mark, we forcibly flag the scene as modified
            if (mCommandMark > mCommandManager.CurrentCommandIndex && mCommandMark >= mCommandManager.Commands.Count)
                Modified = true;

            RefreshRenderPanel();
        }

        /// <summary>
        /// Called whenever the command manager has undone a command
        /// </summary>
        /// <param name="command"></param>
        void mCommandManager_OnUndo(Command command)
        {
            RefreshRenderPanel();
        }
        #endregion
        
        #region Event Handlers : Scene View
        /// <summary>
        /// Mouse wheel was rotated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SceneView_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta < 0)
                Zoom -= 0.05f;
            else
                Zoom += 0.05f;
        }

        /// <summary>
        /// Called when the input focus leaves this control.  Ensure that we reset the next control
        /// to be created on mouse down.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SceneView_Leave(object sender, EventArgs e)
        {
            CreateControlType = null;
            mEnableSnapping = false;
        }

        /// <summary>
        /// Called when the scene has been activated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SceneView_Activated(object sender, EventArgs e)
        {
            UpdateProjectionMatrix();
        }
        #endregion

        #region Event Handlers : Notifying List (Selected Controls)

        /// <summary>
        /// A control has been added to the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="item"></param>
        void mSelectedControls_OnItemAdded(object sender, GUIControl item)
        {
            NotifyControlSelectionChanged(mSelectedControls);
        }

        /// <summary>
        /// A control has been removed from the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="item"></param>
        void mSelectedControls_OnItemRemoved(object sender, GUIControl item)
        {
            NotifyControlSelectionChanged(mSelectedControls);
        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="renderer"></param>
        public SceneView(GUIScene scene)
        {
            PromptSave = true;

            mScene = scene;
            mScene.OnTextureUpdated += new GUIScene.TextureEventHandler(mScene_OnTextureUpdated);

            mCommandManager = new CommandManager();
            mCommandManager.AddCommand(new NullCommand("- Scene -"), false);

            mCommandManager.OnCommandAdded += new CommandManager.CommandDelegate(mCommandManager_OnCommandAdded);
            mCommandManager.OnExecute += new CommandManager.CommandDelegate(mCommandManager_OnExecute);
            mCommandManager.OnUndo += new CommandManager.CommandDelegate(mCommandManager_OnUndo);

            mSelectedControls.OnItemAdded += new NotifyingList<GUIControl>.ListEventHandler(mSelectedControls_OnItemAdded);
            mSelectedControls.OnItemRemoved += new NotifyingList<GUIControl>.ListEventHandler(mSelectedControls_OnItemRemoved);

            InitializeComponent();
        }

        /// <summary>
        /// called when a texture has been updated in the scene
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="textureInfo"></param>
        void mScene_OnTextureUpdated(object sender, UI.Resources.TextureInfo textureInfo)
        {
            if(textureInfo != null)
            {

                // The OnTextureUpdate may come from a different thread, so ensure
                // that we execute RefreshRenderPanel on the thread that this control
                // was created on.
                this.BeginInvoke(new MethodInvoker(() =>
                    {
                        // TODO : Add anything else in here that needs to happen on the main ui thread
                        RefreshRenderPanel();
                    })
                );
            }
        }

        /// <summary>
        /// Called when the sceneView loads
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            // Open the background texture
            // TODO - we need to move this as a global.
            if (Otter.Interface.Graphics.Instance != null)
            {
                mBackgroundTextureID = LoadTextureFromResource("Otter.Editor.img.view_background.png");
            }

            mCameraLocation.X = this.Width / 2;
            mCameraLocation.Y = this.Height / 2;

            SetResolutions(GUIProject.CurrentProject.Resolutions);
            SetPlatforms(GUIProject.CurrentProject.Platforms);

            mResolutionComboBox.SelectedItem = mScene.Resolution;
            mSafeDisplayComboBox.SelectedIndex = 0;
            mZoomTextBox.Text = "100%";

            this.Leave += new EventHandler(SceneView_Leave);
            this.Activated += new EventHandler(SceneView_Activated);

            base.OnLoad(e);
        }

        /// <summary>
        /// Loads all of the resources for the scene, on a separate thread
        /// while displaying a progress bar.
        /// </summary>
        private void LoadResources()
        {
            List<Otter.UI.Resources.Resource> resources = new List<Otter.UI.Resources.Resource>();

            resources.AddRange(mScene.Textures.OfType<Otter.UI.Resources.Resource>());
            resources.AddRange(mScene.Sounds.OfType<Otter.UI.Resources.Resource>());

            if(resources.Count == 0)
                return;

            LoadingForm form = new LoadingForm();

            form.ProgressBar.Minimum = 0;
            form.ProgressBar.Maximum = resources.Count;
            form.ProgressBar.Value = 0;
            form.Status.Text = "";

            form.Action = () =>
                {
                    int cnt = 1;
                    foreach (Otter.UI.Resources.Resource resource in resources)
                    {
                        form.Status.Text = "[" + (cnt++) + "/" + resources.Count + "] : " + resource;

                        Thread thread = new Thread(new ThreadStart(() => { resource.Load(); }));
                        thread.Start();

                        while (thread.IsAlive)
                            Application.DoEvents();

                        form.ProgressBar.Value++;
                    }
                };

            form.ShowDialog();
        }

        /// <summary>
        /// Loads a texture from an embedded resource
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        private int LoadTextureFromResource(string resource)
        {
            Bitmap bmp = Utils.LoadBitmapResource(resource);

            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);

            // Declare an array to hold the bytes of the bitmap.
            int bytes = bmpData.Stride * bmp.Height;
            byte[] rawData = new byte[bytes];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, rawData, 0, bytes);

            int textureID = -1;
            switch (bmp.PixelFormat)
            {
                case PixelFormat.Format24bppRgb:
                    {
                        textureID = Otter.Interface.Graphics.Instance.LoadTexture(rawData, bmp.Width, bmp.Height, 24);
                        break;
                    }
                case PixelFormat.Format32bppRgb:
                case PixelFormat.Format32bppArgb:
                    {
                        textureID = Otter.Interface.Graphics.Instance.LoadTexture(rawData, bmp.Width, bmp.Height, 32);
                        break;
                    }
                default:
                    {
                        Console.WriteLine("Unsupported pixel format: " + resource);
                        break;
                    }
            }

            bmp.UnlockBits(bmpData);

            return textureID;
        }

        /// <summary>
        /// Deletes the specified control from the sceneView
        /// </summary>
        /// <param name="control"></param>
        private void DeleteControls(List<GUIControl> controls)
        {
            if (controls == null || controls.Count == 0)
                return;

            GUIControl[] controlArray = controls.ToArray();
            foreach (GUIControl control in controlArray)
            {
                CommandManager.AddCommand(new DeleteControlCommand(ActiveView, control), true);
            }
        }

        /// <summary>
        /// Called whenever the user is dragging using the mouse.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="oldPt"></param>
        /// <param name="newPt"></param>
        private void HandleMouseDrag(ref PointF oldPt, ref PointF newPt, ref PointF locationOffset, Constraint constraint)
        {
            if (mPointArea == PointArea.None || PrimaryControl == null)
                return;

            PointF oldLoc = PrimaryControl.Location;
            PointF oldCenter = PrimaryControl.Center;
            SizeF oldSize = PrimaryControl.Size;

            PointF newLoc = oldLoc;
            PointF newCenter = oldCenter;
            SizeF newSize = oldSize;

            // NOTE:  After the invert, the transformed points are in the control's
            //        coordinate space where the control is assumed at (0, 0, w, h).
            Vector4 vOld = Vector4.Transform(new Vector4(oldPt.X, oldPt.Y, 0.0f, 1.0f), PrimaryControl.FullTransformInv);
            Vector4 vNew = Vector4.Transform(new Vector4(newPt.X, newPt.Y, 0.0f, 1.0f), PrimaryControl.FullTransformInv);

            RoundVector(ref vOld, 2);
            RoundVector(ref vNew, 2);

            float localDeltaX = (vNew.X - vOld.X);
            float localDeltaY = (vNew.Y - vOld.Y);

            float left = PrimaryControl.Left;
            float right = PrimaryControl.Right;
            float top = PrimaryControl.Top;
            float bottom = PrimaryControl.Bottom;
            float centerx = PrimaryControl.Center.X;
            float centery = PrimaryControl.Center.Y;

            if ((mPointArea & PointArea.Center) != 0)
            {
                PrimaryControl.Center = new PointF(PrimaryControl.Center.X + localDeltaX, PrimaryControl.Center.Y + localDeltaY);
            }
            else
            {
                if ((mPointArea & PointArea.Left) != 0)
                {
                    float delta = vNew.X;
                    left += delta;

                    if (left > right)
                    {
                        left = right;
                    }
                }

                if ((mPointArea & PointArea.Right) != 0)
                {
                    float delta = (vNew.X - PrimaryControl.Size.Width);
                    right += delta;

                    if (right < left)
                        right = left;
                }

                if ((mPointArea & PointArea.Top) != 0)
                {
                    float delta = vNew.Y;
                    top += delta;

                    if (top > bottom)
                    {
                        top = bottom;
                    }
                }

                if ((mPointArea & PointArea.Bottom) != 0)
                {
                    float delta = (vNew.Y - PrimaryControl.Size.Height);
                    bottom += delta;

                    if (bottom < top)
                        bottom = top;
                }
            }

            if ((oldSize == PrimaryControl.Size && oldCenter == PrimaryControl.Center && (mPointArea == PointArea.Body)) || 
                (mPointArea & PointArea.Center) != 0)
            {
                // Get the parent's full transform.  We will use that to determine how the control is positioned,
                // since the control's position is a point on the parent.
                Matrix invertedTransform = PrimaryControl.Parent != null ? PrimaryControl.Parent.FullTransformInv : Matrix.Identity;

                Vector4 localNew = new Vector4(newPt.X, newPt.Y, 0.0f, 1.0f);
                localNew = Vector4.Transform(localNew, invertedTransform);
                RoundVector(ref localNew, 2);

                newLoc = new PointF(localNew.X - locationOffset.X, localNew.Y - locationOffset.Y);
            }

            newSize = new SizeF((right - left), (bottom - top));
            newCenter = new PointF(-left, -top);

            // Apply the constraints after we've determined the new locations and such
            if ((constraint & Constraint.X) != 0)
            {
                newLoc = new PointF(newLoc.X, mControlStartPos.Y);
            }
            else if ((constraint & Constraint.Y) != 0)
            {
                newLoc = new PointF(mControlStartPos.X, newLoc.Y);
            }

            if ((constraint & Constraint.Scale) != 0)
            {
                float ratio = 1.0f;
                if (mControlStartSize.Height != 0)
                    ratio = mControlStartSize.Height / mControlStartSize.Width;

                float prevH = newSize.Height;
                newSize = new SizeF(newSize.Width, newSize.Width * ratio);

                if ((mPointArea & PointArea.Top) != 0)
                {
                    newCenter = new PointF(newCenter.X, newCenter.Y + (newSize.Height - prevH));
                }
            }

            // Apply the new layout, we're going to need the updated values
            // for the transform below
            PrimaryControl.Center = newCenter;
            PrimaryControl.Location = newLoc;
            PrimaryControl.Size = newSize;

            if (mEnableSnapping)
            {
                Vector4 ls = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
                Vector4 le = new Vector4(0.0f, 768.0f, 0.0f, 1.0f);

                int inc = Otter.Editor.Properties.Settings.Default.ViewGridIncrement;

                // find the maximum bounding radius for this control
                float radius = Math.Max(PrimaryControl.Size.Width, PrimaryControl.Size.Height);
                radius += (float)Math.Sqrt(Math.Pow(PrimaryControl.Center.X, 2) + Math.Pow(PrimaryControl.Center.Y, 2));
                radius += inc;

                // Try snapping against the grid, if shown
                if (mShowGrid)
                {
                    // First try against the vertical lines
                    float minx = (int)((PrimaryControl.Location.X - radius) / inc) * inc;
                    float maxx = (int)((PrimaryControl.Location.X + radius + inc) / inc) * inc;
                    for (float x = minx; x <= maxx; x += inc)
                    {
                        ls.X = x;
                        ls.Y = -99999;

                        le.X = x;
                        le.Y = 99999;

                        if (SnapControl(PrimaryControl, mPointArea, ref ls, ref le))
                            break;
                    }

                    // Now against the horizontal
                    float miny = (int)((PrimaryControl.Location.Y - radius) / inc) * inc;
                    float maxy = (int)((PrimaryControl.Location.Y + radius + inc) / inc) * inc;
                    for (float y = miny; y <= maxy; y += inc)
                    {
                        ls.X = -99999;
                        ls.Y = y;

                        le.X = 99999;
                        le.Y = y;

                        if (SnapControl(PrimaryControl, mPointArea, ref ls, ref le))
                            break;
                    }
                }

                SnapToControls(PrimaryControl, ActiveView.Controls);
            }

            float dx = PrimaryControl.Location.X - oldLoc.X;
            float dy = PrimaryControl.Location.Y - oldLoc.Y;
            foreach (GUIControl control in SelectedControls)
            {
                if(control != PrimaryControl)
                    control.Location = new PointF(control.Location.X + dx, control.Location.Y + dy);
            }

            NotifyControlUpdated(PrimaryControl);

            // We have to refresh the panel here, instead of invalidating, because the "OnControlUpdated" event
            // can cause the invalidate the be lost.
            RefreshRenderPanel();
        }

        private bool SnapToControls(GUIControl control, GUIControlCollection controls)
        {
            float radius = Math.Max(control.Size.Width, control.Size.Height);
            radius += (float)Math.Sqrt(Math.Pow(control.Center.X, 2) + Math.Pow(control.Center.Y, 2));
            radius += Otter.Editor.Properties.Settings.Default.ViewGridIncrement;

            Vector4 controlWorldPos = Vector4.Transform(new Vector4(control.Center.X, control.Center.Y, 0.0f, 1.0f), control.FullTransform);

            // Now check against all other controls
            foreach (GUIControl otherControl in controls)
            {
                if (SelectedControls.Contains(otherControl))
                    continue;

                // find the maximum bounding radius for this control
                float sibRadius = Math.Max(otherControl.Size.Width, otherControl.Size.Height);
                sibRadius += (float)Math.Sqrt(Math.Pow(otherControl.Center.X, 2) + Math.Pow(otherControl.Center.Y, 2));

                // See if the circles intersect.  If they do, check snapping
                Vector4 siblingWorldPos = Vector4.Transform(new Vector4(otherControl.Center.X, otherControl.Center.Y, 0.0f, 1.0f), otherControl.FullTransform);

                float dist = (float)(Math.Pow((siblingWorldPos.X - controlWorldPos.X), 2) + Math.Pow((siblingWorldPos.Y - controlWorldPos.Y), 2));
                if (dist <= Math.Pow((radius + sibRadius), 2))
                {
                    // Left Edge
                    Vector4 ls = Vector4.Transform(new Vector4(0.0f, 0.0f, 0.0f, 1.0f), otherControl.FullTransform);
                    Vector4 le = Vector4.Transform(new Vector4(0.0f, otherControl.Size.Height, 0.0f, 1.0f), otherControl.FullTransform);

                    if (SnapControl(control, mPointArea, ref ls, ref le))
                        return true;

                    // Right Edge
                    ls = Vector4.Transform(new Vector4(otherControl.Size.Width, 0.0f, 0.0f, 1.0f), otherControl.FullTransform);
                    le = Vector4.Transform(new Vector4(otherControl.Size.Width, otherControl.Size.Height, 0.0f, 1.0f), otherControl.FullTransform);

                    if (SnapControl(control, mPointArea, ref ls, ref le))
                        return true;

                    // Top Edge
                    ls = Vector4.Transform(new Vector4(0.0f, 0.0f, 0.0f, 1.0f), otherControl.FullTransform);
                    le = Vector4.Transform(new Vector4(otherControl.Size.Width, 0.0f, 0.0f, 1.0f), otherControl.FullTransform);

                    if (SnapControl(control, mPointArea, ref ls, ref le))
                        return true;

                    // Bottom Edge
                    ls = Vector4.Transform(new Vector4(0.0f, otherControl.Size.Height, 0.0f, 1.0f), otherControl.FullTransform);
                    le = Vector4.Transform(new Vector4(otherControl.Size.Width, otherControl.Size.Height, 0.0f, 1.0f), otherControl.FullTransform);

                    if (SnapControl(control, mPointArea, ref ls, ref le))
                        return true;
                }

                if (SnapToControls(control, otherControl.Controls))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Snaps a control to the provided line segment.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="pointArea"></param>
        /// <param name="lineStart"></param>
        /// <param name="lineEnd"></param>
        private bool SnapControl(GUIControl control, PointArea pointArea, ref Vector4 lineStart, ref Vector4 lineEnd)
        {
            PointF newLoc = control.Location;
            PointF newCenter = control.Center;
            SizeF newSize = control.Size;

            // Transform the snap line to the control's coordinate space, that way our math is slightly
            // easier to manage

            Matrix transform = control.FullTransform;
            Matrix invert = control.FullTransformInv;

            Vector4 ls = Vector4.Transform(lineStart, invert);
            Vector4 le = Vector4.Transform(lineEnd, invert);

            PointF lv = new PointF(le.X - ls.X, le.Y - ls.Y);
            float llen = (float)Math.Sqrt(lv.X * lv.X + lv.Y * lv.Y);
            PointF ln = new PointF(lv.X / llen, lv.Y / llen);

            int tolerance = Otter.Editor.Properties.Settings.Default.ViewSnapTolerance;

            // If the line is vertical or horizontal, we have an eas(ier) case,
            // and we can do edge-snapping too.
            if (ln.X == 0.0f && ln.Y == 1.0f || ln.X == 1.0f && ln.Y == 0.0f)
            {
                PointF delta = new PointF(0.0f, 0.0f);
                bool bSnapped = false;

                // Left / Right Edge
                if (ln.Y == 1.0f && ls.Y <= control.Size.Height && le.Y >= 0.0f)
                {
                    // Left Edge
                    if (Math.Abs(ls.X) < tolerance)
                    {
                        if ((pointArea & PointArea.Left) != 0)
                        {
                            newSize = new SizeF(newSize.Width - ls.X, newSize.Height);
                            newCenter = new PointF(newCenter.X - ls.X, newCenter.Y);
                        }
                        else
                        {
                            newLoc = new PointF(newLoc.X + ls.X, newLoc.Y);
                        }

                        bSnapped = true;
                    }
                    // Right Edge
                    else if (Math.Abs(ls.X - control.Size.Width) < tolerance)
                    {
                        if ((pointArea & PointArea.Right) != 0)
                        {
                            newSize = new SizeF(newSize.Width + (ls.X - control.Size.Width), newSize.Height);
                        }
                        else
                        {
                            newLoc = new PointF(newLoc.X + ls.X - control.Size.Width, newLoc.Y);
                        }

                        bSnapped = true;
                    }
                }

                // Top / Bottom Edge
                if (ln.X == 1.0f && ls.X <= control.Size.Width && le.X >= 0.0f)
                {
                    // Top Edge
                    if (Math.Abs(ls.Y) < tolerance)
                    {
                        if ((pointArea & PointArea.Top) != 0)
                        {
                            newSize = new SizeF(newSize.Width, newSize.Height - ls.Y);
                            newCenter = new PointF(newCenter.X, newCenter.Y - ls.Y);
                        }
                        else
                        {
                            newLoc = new PointF(newLoc.X, newLoc.Y + ls.Y);
                        }

                        bSnapped = true;
                    }
                    // Bottom Edge
                    else if (Math.Abs(ls.Y - control.Size.Height) < tolerance)
                    {
                        if ((pointArea & PointArea.Bottom) != 0)
                        {
                            newSize = new SizeF(newSize.Width, newSize.Height + (ls.Y - control.Size.Height));
                        }
                        else
                        {
                            newLoc = new PointF(newLoc.X, newLoc.Y + (ls.Y - control.Size.Height));
                        }

                        bSnapped = true;
                    }
                }

                if (bSnapped)
                {
                    control.Center = newCenter;
                    control.Location = newLoc;
                    control.Size = newSize;
                }

                return bSnapped;
            }

            // Get the corners of the control's bounds
            for (int i = 0; i < 4; i++)
            {
                PointF vertex = PointF.Empty;
                bool bContinue = false;

                switch (i)
                {
                    // Top Left
                    case 0:
                        {
                            if ((pointArea & PointArea.Body) != 0 ||
                               (pointArea & PointArea.Top) != 0 && (pointArea & PointArea.Left) != 0)
                            {
                                vertex = new PointF(0.0f, 0.0f);
                                bContinue = true;
                            }

                            break;
                        }
                    // Top Right
                    case 1:
                        {
                            if ((pointArea & PointArea.Body) != 0 ||
                               (pointArea & PointArea.Top) != 0 && (pointArea & PointArea.Right) != 0)
                            {
                                vertex = new PointF(newSize.Width, 0.0f);
                                bContinue = true;
                            }
                            break;
                        }
                    // Bottom Left
                    case 2:
                        {
                            if ((pointArea & PointArea.Body) != 0 ||
                               (pointArea & PointArea.Bottom) != 0 && (pointArea & PointArea.Left) != 0)
                            {
                                vertex = new PointF(0.0f, newSize.Height);
                                bContinue = true;
                            }
                            break;
                        }
                    // Bottom Right
                    case 3:
                        {

                            if ((pointArea & PointArea.Body) != 0 ||
                               (pointArea & PointArea.Bottom) != 0 && (pointArea & PointArea.Right) != 0)
                            {
                                vertex = new PointF(newSize.Width, newSize.Height);
                                bContinue = true;
                            }
                            break;
                        }
                }

                if (!bContinue)
                    continue;

                // Let's snap the top left for now, and then see if we can make everything else "fit"
                PointF toVertex = new PointF(vertex.X - ls.X, vertex.Y - ls.Y);
                float toVertexLen = (float)Math.Sqrt(toVertex.X * toVertex.X + toVertex.Y * toVertex.Y);
                PointF toVertexNormal = new PointF(toVertex.X / toVertexLen, toVertex.Y / toVertexLen);

                float dot = toVertex.X * ln.X + toVertex.Y * ln.Y;

                if (dot >= 0.0f && dot <= llen)
                {
                    PointF closestPt = new PointF(ls.X + ln.X * dot, ls.Y + ln.Y * dot);
                    PointF toClosestPt = new PointF(closestPt.X - vertex.X, closestPt.Y - vertex.Y);
                    float dist = (float)Math.Sqrt(toClosestPt.X * toClosestPt.X + toClosestPt.Y * toClosestPt.Y);

                    if (dist <= tolerance)
                    {
                        // We need to snap
                        Vector4 tmp = Vector4.Transform(new Vector4(vertex.X, vertex.Y, 0.0f, 1.0f), transform);
                        Vector4 tgt = Vector4.Transform(new Vector4(closestPt.X, closestPt.Y, 0.0f, 1.0f), transform);

                        // Ok, we have the snap location .. now to figure out what to do with this thing.

                        // Simple case - dragging the entire body area.  Just offset the location.
                        if ((pointArea & PointArea.Body) != 0)
                        {
                            PointF delta = new PointF(tgt.X - tmp.X, tgt.Y - tmp.Y);
                            newLoc = new PointF(newLoc.X + delta.X, newLoc.Y + delta.Y);
                        }
                        else
                        {
                            // Since we've calculated the closest point in the control's space,
                            // it's easier to manipulate the size / center etc using the untransformed
                            // points
                            PointF delta = new PointF(closestPt.X - vertex.X, closestPt.Y - vertex.Y);
                            switch (i)
                            {
                                // Top Left
                                case 0:
                                    {
                                        newSize = new SizeF(newSize.Width - delta.X, newSize.Height - delta.Y);
                                        newCenter = new PointF(newCenter.X - delta.X, newCenter.Y - delta.Y);
                                        break;
                                    }
                                // Top Right
                                case 1:
                                    {
                                        newSize = new SizeF(newSize.Width + delta.X, newSize.Height - delta.Y);
                                        newCenter = new PointF(newCenter.X, newCenter.Y - delta.Y);
                                        break;
                                    }
                                // Bottom Left
                                case 2:
                                    {
                                        newSize = new SizeF(newSize.Width - delta.X, newSize.Height + delta.Y);
                                        newCenter = new PointF(newCenter.X - delta.X, newCenter.Y);
                                        break;
                                    }
                                // Bottom Right
                                case 3:
                                    {
                                        newSize = new SizeF(newSize.Width + delta.X, newSize.Height + delta.Y);
                                        break;
                                    }
                            }
                        }

                        control.Center = newCenter;
                        control.Location = newLoc;
                        control.Size = newSize;

                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Rounds a vector's components to the nearest decimal, as specified
        /// </summary>
        /// <param name="vec"></param>
        private static void RoundVector(ref Vector4 vec, int digits)
        {
            vec.X = (float)Math.Round(vec.X, digits);
            vec.Y = (float)Math.Round(vec.Y, digits);
            vec.Z = (float)Math.Round(vec.Z, digits);
            vec.W = (float)Math.Round(vec.W, digits);
        }

        /// <summary>
        /// Draws the background
        /// </summary>
        private void DrawBackground()
        {
            Color color = Otter.Editor.Properties.Settings.Default.ViewBackgroundColor;

            if (Otter.Editor.Properties.Settings.Default.ViewBackgroundMode == Otter.Editor.BackgroundMode.Color)
            {
                Otter.Interface.Graphics.Instance.DrawRectangle(-1, 0, 0, mScene.Resolution.Width, mScene.Resolution.Height, color.ToArgb());
            }
            else
            {
                int viewWidth = mScene.Resolution.Width;
                int viewHeight = mScene.Resolution.Height;
                int texWidth = 512;
                int texHeight = 512;

                for (int x = 0; x < viewWidth; x += texWidth)
                {
                    for (int y = 0; y < viewHeight; y += texHeight)
                    {
                        int w = (viewWidth - x) > texWidth ? texWidth : (viewWidth - x);
                        int h = (viewHeight - y) > texHeight ? texHeight : (viewHeight - y);

                        float u = w / (float)texWidth;
                        float v = h / (float)texHeight;

                        Triangle[] triangles = new Triangle[2];
                        triangles[0] = new Triangle();
                        triangles[0].SetVertex(0, x, y, 0.0f, 0.0f, 0.0f, Color.White.ToArgb());
                        triangles[0].SetVertex(1, x + w, y, 0.0f, u, 0.0f, Color.White.ToArgb());
                        triangles[0].SetVertex(2, x + w, y + h, 0.0f, u, v, Color.White.ToArgb());

                        triangles[1] = new Triangle();
                        triangles[1].SetVertex(0, x + w, y + h, 0.0f, u, v, Color.White.ToArgb());
                        triangles[1].SetVertex(1, x, y + h, 0.0f, 0.0f, v, Color.White.ToArgb());
                        triangles[1].SetVertex(2, x, y, 0.0f, 0.0f, 0.0f, Color.White.ToArgb());

                        Otter.Interface.Graphics.Instance.DrawTriangles(mBackgroundTextureID, triangles);
                    }
                }
            }
        }

        /// <summary>
        /// Draws a box highlighting the control
        /// </summary>
        /// <param name="control"></param>
        /// <param name="color"></param>
        private void DrawHighlightBox(GUIControl control, bool drawGrips, Color color, Color color2)
        {
            if (control == null)
                return;

            Matrix matrix = control.FullTransform;

            Otter.Interface.Graphics.Instance.PushMatrix(matrix.Entries);
            {
                float w = control.Size.Width;
                float h = control.Size.Height; 
                float gw = 8; // Grip Width

                if (drawGrips)
                {
                    //------------------------------------------
                    // Grips (filled)
                    //------------------------------------------

                    // Grips
                    Otter.Interface.Graphics.Instance.DrawRectangle(-1, -gw, -gw, gw, gw, color2.ToArgb()); // Top Left
                    Otter.Interface.Graphics.Instance.DrawRectangle(-1, (control.Size.Width / 2) - 4, -gw, gw, gw, color2.ToArgb()); // Top
                    Otter.Interface.Graphics.Instance.DrawRectangle(-1, control.Size.Width, -gw, gw, gw, color2.ToArgb()); // Top Right
                    Otter.Interface.Graphics.Instance.DrawRectangle(-1, -gw, (control.Size.Height / 2) - 4, gw, gw, color2.ToArgb()); // Left
                    Otter.Interface.Graphics.Instance.DrawRectangle(-1, control.Size.Width, (control.Size.Height / 2) - 4, gw, gw, color2.ToArgb()); // Right
                    Otter.Interface.Graphics.Instance.DrawRectangle(-1, -gw, control.Size.Height, gw, gw, color2.ToArgb()); // Bottom Left
                    Otter.Interface.Graphics.Instance.DrawRectangle(-1, (control.Size.Width / 2) - 4, control.Size.Height, gw, gw, color2.ToArgb()); // Bottom
                    Otter.Interface.Graphics.Instance.DrawRectangle(-1, control.Size.Width, control.Size.Height, gw, gw, color2.ToArgb()); // Bottom Right

                    //------------------------------------------
                    // Grips (outlines) and frame
                    //------------------------------------------

                    // Draw the frame
                    // Top Horizontal
                    Otter.Interface.Graphics.Instance.DrawLine(-gw, 0, 0, w + gw, 0, 0, color.ToArgb());

                    // Top Left Corner
                    Otter.Interface.Graphics.Instance.DrawLine(-gw, -gw, 0, 0, -gw, 0, color.ToArgb());
                    Otter.Interface.Graphics.Instance.DrawLine(-gw, -gw, 0, -gw, 0, 0, color.ToArgb());

                    // Top
                    Otter.Interface.Graphics.Instance.DrawLine((control.Size.Width / 2) - 4, -gw, 0, (control.Size.Width / 2) + 4, -gw, 0, color.ToArgb());
                    Otter.Interface.Graphics.Instance.DrawLine((control.Size.Width / 2) - 4, -gw, 0, (control.Size.Width / 2) - 4, 0, 0, color.ToArgb());
                    Otter.Interface.Graphics.Instance.DrawLine((control.Size.Width / 2) + 4, -gw, 0, (control.Size.Width / 2) + 4, 0, 0, color.ToArgb());

                    // Right Vertical
                    Otter.Interface.Graphics.Instance.DrawLine(w, -gw, 0, w, h + gw, 0, color.ToArgb());

                    // Top Right Corner
                    Otter.Interface.Graphics.Instance.DrawLine(w + gw, -gw, 0, w, -gw, 0, color.ToArgb());
                    Otter.Interface.Graphics.Instance.DrawLine(w + gw, -gw, 0, w + gw, 0, 0, color.ToArgb());

                    // Right
                    Otter.Interface.Graphics.Instance.DrawLine(w, (control.Size.Height / 2) - 4, 0, w + gw, (control.Size.Height / 2) - 4, 0, color.ToArgb());
                    Otter.Interface.Graphics.Instance.DrawLine(w, (control.Size.Height / 2) + 4, 0, w + gw, (control.Size.Height / 2) + 4, 0, color.ToArgb());
                    Otter.Interface.Graphics.Instance.DrawLine(w + gw, (control.Size.Height / 2) - 4, 0, w + gw, (control.Size.Height / 2) + 4, 0, color.ToArgb());

                    // Bottom Horizontal
                    Otter.Interface.Graphics.Instance.DrawLine(-gw, h, 0, w + gw, h, 0, color.ToArgb());

                    // Bottom Right Corner
                    Otter.Interface.Graphics.Instance.DrawLine(w + gw, h + gw, 0, w, h + gw, 0, color.ToArgb());
                    Otter.Interface.Graphics.Instance.DrawLine(w + gw, h + gw, 0, w + gw, h, 0, color.ToArgb());

                    // Bottom
                    Otter.Interface.Graphics.Instance.DrawLine((control.Size.Width / 2) - 4, h + gw, 0, (control.Size.Width / 2) + 4, h + gw, 0, color.ToArgb());
                    Otter.Interface.Graphics.Instance.DrawLine((control.Size.Width / 2) - 4, h, 0, (control.Size.Width / 2) - 4, h + gw, 0, color.ToArgb());
                    Otter.Interface.Graphics.Instance.DrawLine((control.Size.Width / 2) + 4, h, 0, (control.Size.Width / 2) + 4, h + gw, 0, color.ToArgb());

                    // Left Vertical
                    Otter.Interface.Graphics.Instance.DrawLine(0, -gw, 0, 0, h + gw, 0, color.ToArgb());

                    // Bottom Left Corner
                    Otter.Interface.Graphics.Instance.DrawLine(-gw, h + gw, 0, 0, h + gw, 0, color.ToArgb());
                    Otter.Interface.Graphics.Instance.DrawLine(-gw, h + gw, 0, -gw, h, 0, color.ToArgb());

                    // Left
                    Otter.Interface.Graphics.Instance.DrawLine(-gw, (control.Size.Height / 2) - 4, 0, 0, (control.Size.Height / 2) - 4, 0, color.ToArgb());
                    Otter.Interface.Graphics.Instance.DrawLine(-gw, (control.Size.Height / 2) + 4, 0, 0, (control.Size.Height / 2) + 4, 0, color.ToArgb());
                    Otter.Interface.Graphics.Instance.DrawLine(-gw, (control.Size.Height / 2) - 4, 0, -gw, (control.Size.Height / 2) + 4, 0, color.ToArgb());

                    //---------------------------------
                    // Center (filled)
                    //---------------------------------
                    float x = control.Center.X;
                    float y = control.Center.Y;

                    Triangle[] triangles = new Triangle[2];
                    triangles[0] = new Triangle();
                    triangles[0].SetVertex(0, x - 6, y, 0.0f, 0.0f, 0.0f, color2.ToArgb());
                    triangles[0].SetVertex(1, x, y - 6, 0.0f, 0.0f, 0.0f, color2.ToArgb());
                    triangles[0].SetVertex(2, x + 6, y, 0.0f, 0.0f, 0.0f, color2.ToArgb());

                    triangles[1] = new Triangle();
                    triangles[1].SetVertex(0, x - 6, y, 0.0f, 0.0f, 0.0f, color2.ToArgb());
                    triangles[1].SetVertex(1, x + 6, y, 0.0f, 0.0f, 0.0f, color2.ToArgb());
                    triangles[1].SetVertex(2, x, y + 6, 0.0f, 0.0f, 0.0f, color2.ToArgb());

                    Otter.Interface.Graphics.Instance.DrawTriangles(-1, triangles);

                    //---------------------------------
                    // Center (outlines)
                    //---------------------------------
                    Otter.Interface.Graphics.Instance.DrawLine(x - 6, y, 0, x, y - 6, 0, color.ToArgb());
                    Otter.Interface.Graphics.Instance.DrawLine(x, y - 6, 0, x + 6, y, 0, color.ToArgb());
                    Otter.Interface.Graphics.Instance.DrawLine(x + 6, y, 0, x, y + 6, 0, color.ToArgb());
                    Otter.Interface.Graphics.Instance.DrawLine(x, y + 6, 0, x - 6, y, 0, color.ToArgb());
                }
                else
                {
                    Otter.Interface.Graphics.Instance.DrawLine(0, 0, 0, w, 0, 0, color.ToArgb());
                    Otter.Interface.Graphics.Instance.DrawLine(0, h, 0, w, h, 0, color.ToArgb());
                    Otter.Interface.Graphics.Instance.DrawLine(0, 0, 0, 0, h, 0, color.ToArgb());
                    Otter.Interface.Graphics.Instance.DrawLine(w, 0, 0, w, h, 0, color.ToArgb());
                }
            }
            Otter.Interface.Graphics.Instance.PopMatrix();
        }

        /// <summary>
        /// Fills the resolution combo box with data
        /// </summary>
        public void SetResolutions(List<Resolution> resolutions)
        {
            Resolution currentResolution = mResolutionComboBox.SelectedItem as Resolution;
            mResolutionComboBox.Items.Clear();

            foreach (Resolution resolution in resolutions)
            {
                mResolutionComboBox.Items.Add(resolution);
            }

            if (currentResolution != null)
            {
                if (mResolutionComboBox.Items.Contains(currentResolution))
                    mResolutionComboBox.SelectedItem = currentResolution;
                else
                    mResolutionComboBox.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Fills the platforms combo boxes with data
        /// </summary>
        public void SetPlatforms(List<Platform> platforms)
        {
            Platform currentPlatform = mSafeDisplayComboBox.SelectedItem as Platform;
            mSafeDisplayComboBox.Items.Clear();
            mSafeDisplayComboBox.Items.Add("(none)");

            foreach (Platform platform in platforms)
            {
                mSafeDisplayComboBox.Items.Add(platform);
            }

            if (currentPlatform != null && mSafeDisplayComboBox.Items.Contains(currentPlatform))
                mSafeDisplayComboBox.SelectedItem = currentPlatform;
            else
                mSafeDisplayComboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Refreshes the render panel, causing itself to re-render.
        /// </summary>
        public void RefreshRenderPanel()
        {
            mRenderPanel.Refresh();
        }

        /// <summary>
        /// Updates the projection matrix
        /// </summary>
        private void UpdateProjectionMatrix()
        {
            float zoomFactor = 1.0f / Zoom;
            float halfW = (mRenderPanel.Width / 2.0f) * zoomFactor;
            float halfH = (mRenderPanel.Height / 2.0f) * zoomFactor;

            Projection projection = new Projection(-halfW, halfW, -halfH, halfH, 0.1f, 10000.0f);
            Otter.Interface.Graphics.Instance.SetOrtho(projection);
        }

        /// <summary>
        /// Updates the sceneView matrix
        /// </summary>
        private void UpdateViewMatrix()
        {
            Otter.Interface.Graphics.Instance.SetLookAtLH(  mCameraLocation.X, mCameraLocation.Y, -1000,
                                                            mCameraLocation.X, mCameraLocation.Y, 1000,
                                                            0, 1, 0);
        }

        /// <summary>
        /// Converts a client-relative point to a viewport-relative point
        /// </summary>
        /// <param name="point"></param>
        private PointF ClientToViewport(Point point)
        {
            float x = point.X; // *(Otter.Interface.Graphics.Instance.GetWidth() / (float)mRenderPanel.Width);
            float y = point.Y; // *(Otter.Interface.Graphics.Instance.GetHeight() / (float)mRenderPanel.Height);
            float z = 0.0f;

            Otter.Interface.Graphics.Instance.Unproject(ref x, ref y, ref z);
            return new PointF(x, y);
        }

        /// <summary>
        /// Converts a point on the viewport to a local point on the gui control
        /// </summary>
        /// <param name="point"></param>
        /// <param name="control"></param>
        /// <returns></returns>
        private PointF ViewportToControl(PointF point, GUIControl control)
        {
            if (control == null)
                return PointF.Empty;

            Vector4 v = Vector4.Transform(new Vector4(point.X, point.Y, 0.0f, 1.0f), control.FullTransformInv);
            return new PointF(v.X, v.Y);
        }

        /// <summary>
        /// Determines a unique control name
        /// </summary>
        /// <returns></returns>
        private string GetUniqueControlName(GUIControl control)
        {
            int cnt = 1;
            while (ActiveView.Controls.GetControl(control.ToString() + " " + cnt) != null)
                cnt++;

            return control.ToString() + " " + cnt;
        }

        /// <summary>
        /// Called when the user clicked on any of the bounds buttons
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showScreenBoundsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Console.WriteLine("sender == " + sender);

            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if (menuItem == hideScreenBoundsToolStripMenuItem)
            {
                mScreenBoundsColor = Color.White;
            }
            else if (menuItem == showScreenBoundsRedToolStripMenuItem)
            {
                mScreenBoundsColor = Color.Red;
            }
            else if (menuItem == showScreenBoundsTransparentToolStripMenuItem)
            {
                mScreenBoundsColor = Color.FromArgb(50, Color.White);
            }

            mBoundsToolStripDropDownButton.Image = menuItem.Image;
            RefreshRenderPanel();
        }

        private void mSafeDisplayComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            mShowPlatformSafety = mSafeDisplayComboBox.SelectedItem as Platform;
            RefreshRenderPanel();
        }

        /// <summary>
        /// Enables / Disables the grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mShowGridToolStripButton_Click(object sender, EventArgs e)
        {
            mShowGrid = mShowGridToolStripButton.Checked = !mShowGridToolStripButton.Checked;
            
            RefreshRenderPanel();
        }
    }
}
