using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Linq;

using Otter.UI;
using Otter.UI.Animation;
using Otter.Commands;
using Otter.Editor.Commands;
using Otter.Editor.Commands.AnimationCommands;
using Otter.Editor.Forms;
using Otter.UI.Actions;
using Otter.Containers;

namespace Otter.Editor.CustomControls
{
    /// <summary>
    /// Displays and manipulates an channel.
    /// </summary>
    public partial class TimelineControl : UserControl
    {
        #region Types
        public enum Filter { All, Animated }

        private class TimelineChannel
        {
            public GUIControl Control { get; set; }
            public GUIAnimationChannel Channel { get; set; }
            public int Indent { get; set; }
        }
        #endregion

        #region Events and Delegates
        public delegate void TimelineControlEventHandler(object sender);

        public event TimelineControlEventHandler CurrentFrameChanged = null;
        public event TimelineControlEventHandler AnimationChanged = null;
        public event TimelineControlEventHandler AnimationUpdated = null;
        public event TimelineControlEventHandler SelectedControlsChanged = null;
        public event TimelineControlEventHandler FrameSelectionChanged = null;
        #endregion

        #region Data
        private Rectangle mFrameGridRectangle = Rectangle.Empty;
        private GUIView mView = null;
        private GUIAnimation mAnimation = null;
        private List<TimelineChannel> mChannels = new List<TimelineChannel>();

        private Rectangle mFrameRect = new Rectangle(0, 0, 8, 16);
        private Point mLastMousePos = Point.Empty;
        private bool mDraggingIndicator = false;
        private bool mDraggingNamesWidth = false;
        private bool mCreatingSelection = false;
        private bool mDraggingSelection = false;
        private int mNamesRectWidth = 200;

        private int mSelectedChannel = -1;
        private int mSelectedFrameStart = -1;
        private int mSelectedFrameEnd = -1;
        private NotifyingList<GUIControl> mSelectedControls = new NotifyingList<GUIControl>();

        private Font mFrameNumberFont = null;

        private List<KeyFrame> mCopiedKeyFrames = new List<KeyFrame>();

        private Filter mFilter = Filter.All;

        private GUIAnimation.AnimationEventHandler mFrameCountChangedEventHandler = null;

        private GUIAnimation.AnimationEventHandler mKeyFrameAddedEventHandler = null;
        private GUIAnimation.AnimationEventHandler mKeyFrameRemovedEventHandler = null;

        private GUIAnimation.AnimationChannelEventHandler mAnimationChannelAddedEventHandler = null;
        private GUIAnimation.AnimationChannelEventHandler mAnimationChannelRemovedEventHandler = null;

        private Rectangle mNamesRectangle = Rectangle.Empty;
        private Rectangle mChannelsRect = Rectangle.Empty;
        private Rectangle mRulerRect = Rectangle.Empty;
        private Rectangle mMainChannelRect = Rectangle.Empty;
        private Rectangle mSelectRect = Rectangle.Empty;

        private Rectangle mChannelsClipRect = Rectangle.Empty;
        private Rectangle mNamesClipRect = Rectangle.Empty;
        private Rectangle mMainChannelClipRect = Rectangle.Empty;
        private Rectangle mRulerClipRect = Rectangle.Empty;

        private CommandManager mCommandManager = null;
        #endregion

        #region Properties
        /// <summary>
        /// Gets / Sets the sceneView
        /// </summary>
        public GUIView View
        {
            get { return mView; }
            set 
            {
                if (mView != value)
                {
                    if (mView != null)
                    {
                        SetHandlers(mView, false);
                    }

                    mView = value;
                    Animation = (mView == null) ? null : mView.CurrentAnimation;


                    if(mView != null)
                    {
                        SetHandlers(mView, true);
                    }
                }
            }
        }
       
        /// <summary>
        /// Gets / Sets the current channel set.
        /// </summary>
        public GUIAnimation Animation
        {
            get { return mAnimation; }
            set 
            {
                if (mAnimation != value)
                {
                    if (mAnimation != null)
                    {
                        mAnimation.FrameCountChanged -= mFrameCountChangedEventHandler;
                        mAnimation.KeyframeAdded -= mKeyFrameAddedEventHandler;
                        mAnimation.KeyframeRemoved -= mKeyFrameRemovedEventHandler;
                        mAnimation.AnimationChannelAdded -= mAnimationChannelAddedEventHandler;
                        mAnimation.AnimationChannelRemoved -= mAnimationChannelRemovedEventHandler;
                    }

                    mAnimation = value;

                    if (mAnimation != null)
                    {
                        mAnimation.FrameCountChanged += mFrameCountChangedEventHandler;
                        mAnimation.KeyframeAdded += mKeyFrameAddedEventHandler;
                        mAnimation.KeyframeRemoved += mKeyFrameRemovedEventHandler;
                        mAnimation.AnimationChannelAdded += mAnimationChannelAddedEventHandler;
                        mAnimation.AnimationChannelRemoved += mAnimationChannelRemovedEventHandler;
                    }

                    mChannels = GetChannelList();

                    ResetSelection();

                    mHScrollBar.Value = 0;
                    mVScrollBar.Value = 0;

                    UpdateFrameGridRect();
                    UpdateScrollBars();
                    UpdateRectangles();

                    NotifyAnimationChanged();

                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets / Sets the current frame
        /// </summary>
        public UInt32 CurrentFrame
        {
            get 
            {
                if (mAnimation == null)
                    return 0;

                return mAnimation.Frame; ; 
            }
            set 
            {
                if (mAnimation == null)
                    return;

                if (mAnimation.Frame != value)
                {
                    mAnimation.Frame = value;
                    NotifyFrameChanged();

                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets the currentframe indicator rectangle
        /// </summary>
        private Rectangle CurrentFrameRect
        {
            get
            {
                int w = 20;
                int h = 10;
                int x = mFrameGridRectangle.Left + mFrameRect.Width * (int)CurrentFrame + mFrameRect.Width / 2 - w / 2;
                int y = mFrameGridRectangle.Top - h;
                return new Rectangle(x, y, w, h);
            }
        }

        /// <summary>
        /// Gets / Sets the channel filter
        /// </summary>
        public Filter ChannelFilter
        {
            get { return mFilter; }
            set 
            {
                mFilter = value;
                mChannels = GetChannelList();

                UpdateFrameGridRect();
                UpdateScrollBars();
                UpdateRectangles();

                Invalidate();
            }
        }

        /// <summary>
        /// Retrieves the currently selected channel index.
        /// 0     => Main Channel
        /// 1 - N => 1-based channel channel index
        /// </summary>
        public int SelectedChannel
        {
            get { return mSelectedChannel; }
        }

        /// <summary>
        /// Retrieves the currently selected frame start
        /// </summary>
        public int SelectedFrameStart
        {
            get { return mSelectedFrameStart; }
        }

        /// <summary>
        /// Retrieves the currently selected frame end
        /// </summary>
        public int SelectedFrameEnd
        {
            get { return mSelectedFrameEnd; }
        }
        
        /// <summary>
        /// Gets / Sets the selected control
        /// </summary>
        public NotifyingList<GUIControl> SelectedControls
        {
            get { return mSelectedControls; }
            set
            {
                NotifyingList<GUIControl> list = value;
                if (list == null)
                    list = new NotifyingList<GUIControl>();

                List<GUIControl> intersection = new List<GUIControl>(mSelectedControls.Intersect(list));
                if (mSelectedControls.Count == list.Count && intersection.Count == list.Count)
                    return;

                if (SelectedChannel >= 0)
                    ResetSelection();

                mSelectedControls.SuppressEvents = true;
                {
                    mSelectedControls.Clear();
                    if (list != null)
                        mSelectedControls.AddRange(list.Where(a => !(a is GUIView)));
                }
                mSelectedControls.SuppressEvents = false;

                NotifySelectedControlChanged();
                this.Invalidate();
            }
        }

        /// <summary>
        /// Retrieves a list of selected keyframes
        /// </summary>
        public List<BaseFrame> SelectedFrames
        {
            get
            {
                List<BaseFrame> frames = new List<BaseFrame>();

                if (SelectedChannel == 0)
                {
                    for (int i = SelectedFrameStart; i <= SelectedFrameEnd; i++)
                    {
                        MainChannelFrame mainChannelFrame = mAnimation.GetMainChannelFrame((uint)i);
                        if (mainChannelFrame != null)
                            frames.Add(mainChannelFrame);
                        else
                        {
                            // Create a temporary frame and add it to the animation.  
                            // The frame will add / remove itself from the animation appropriately.
                            MainChannelFrame frame = new MainChannelFrame();
                            frame.Frame = (uint)i;
                            frame.Animation = this.Animation;
                            frames.Add(frame);
                        }

                    }
                }
                else
                {
                    int index = SelectedChannel - 1;
                    if (index < 0 || index >= mChannels.Count)
                        return frames;

                    for (int i = SelectedFrameStart; i <= SelectedFrameEnd; i++)
                    {
                        GUIAnimationChannel channel = mChannels[index].Channel;
                        if (channel == null)
                            continue;

                        KeyFrame frame = channel.GetKeyFrame(i);

                        if (frame != null)
                            frames.Add(frame);
                    }
                }

                return frames;
            }
        }

        /// <summary>
        /// Gets / Sets the command manager for the control
        /// </summary>
        public CommandManager CommandManager
        {
            get { return mCommandManager; }
            set
            {
                if (mCommandManager != value)
                {
                    if (mCommandManager != null)
                    {
                        mCommandManager.OnExecute -= new Otter.Commands.CommandManager.CommandDelegate(mCommandManager_OnExecute);
                        mCommandManager.OnUndo -= new Otter.Commands.CommandManager.CommandDelegate(mCommandManager_OnUndo);
                    }

                    mCommandManager = value;

                    if (mCommandManager != null)
                    {
                        mCommandManager.OnExecute += new Otter.Commands.CommandManager.CommandDelegate(mCommandManager_OnExecute);
                        mCommandManager.OnUndo += new Otter.Commands.CommandManager.CommandDelegate(mCommandManager_OnUndo);
                    }
                }
            }
        }
        #endregion

        #region Event Handlers : Command Manager
        /// <summary>
        /// Called when a command has executed
        /// </summary>
        /// <param name="command"></param>
        void mCommandManager_OnExecute(Command command)
        {
            if (command is ClearFramesCommand)
            {
                // Refresh the channel list in case it was modified
                mChannels = GetChannelList();

                mAnimation.UpdateAnimations();
                NotifyAnimationUpdated();
            }

            this.Invalidate();
        }

        /// <summary>
        /// Called when a command has been undone
        /// </summary>
        /// <param name="command"></param>
        void mCommandManager_OnUndo(Command command)
        {
            if (command is ClearFramesCommand)
            {
                // Refresh the channel list in case it was modified
                mChannels = GetChannelList();

                mAnimation.UpdateAnimations();
                NotifyAnimationUpdated();
            }

            this.Invalidate();
        }
        #endregion

        #region Event Handlers : Mouse
        /// <summary>
        /// Mouse button has been clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimelineControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && mAnimation != null)
            {
                Point pt = ClientToNamesList(e.Location);
                int diff = (mNamesRectWidth - pt.X);

                if (diff >= 0 && diff <= 5)
                {
                    Cursor.Current = Cursors.SizeWE;
                    mDraggingNamesWidth = true;
                }
                else if (mNamesRectangle.Contains(pt))
                {
                    // Determine which control we clicked on
                    int index = (pt.Y - mNamesRectangle.Top) / mFrameRect.Height;
                    GUIControl control = (index < mChannels.Count) ? mChannels[index].Control : null;

                    if (Control.ModifierKeys != Keys.Control)
                        SelectedControls.Clear();

                    if (control != null)
                        SelectedControls.Add(control);

                    return;
                }

                pt = ClientToTimeline(e.Location);
                if (CurrentFrameRect.Y <= pt.Y && CurrentFrameRect.Y + CurrentFrameRect.Height >= pt.Y) //  CurrentFrameRect.Contains(pt))
                {
                    mDraggingIndicator = true;
                    Cursor.Current = Cursors.SizeWE;

                    MoveFrameIndicator(ClientToTimeline(pt));
                    Invalidate(true);
                }
                // See if we're in the selection rectangle
                else if (mSelectRect.Contains(e.Location))
                {
                    mDraggingSelection = true;
                }
                else
                {
                    // Determine which frame we clicked on
                    if (pt.X >= mFrameGridRectangle.Left && pt.Y >= mFrameGridRectangle.Top)
                    {
                        int channel = (pt.Y - mFrameGridRectangle.Top) / mFrameRect.Height;
                        int start = (pt.X - mFrameGridRectangle.Left) / mFrameRect.Width;
                        int end = start;

                        SetSelection(channel, start, end);

                        mCreatingSelection = true;

                        if (SelectedChannel > mChannels.Count || SelectedFrameStart >= mAnimation.NumFrames)
                        {
                            ResetSelection();
                            mCreatingSelection = false;
                        }
                    }
                    else
                    {
                        ResetSelection();
                        mCreatingSelection = false;
                    }

                    UpdateSelectRect();
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Mouse button has been released
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimelineControl_MouseUp(object sender, MouseEventArgs e)
        {
            mDraggingIndicator = false;
            mDraggingNamesWidth = false;
            mCreatingSelection = false; 
            mDraggingSelection = false;
        }  

        /// <summary>
        /// Called while the mouse is moving across the control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimelineControl_MouseMove(object sender, MouseEventArgs e)
        {
            Point timelinePoint = ClientToTimeline(e.Location);
            Point namesPoint = ClientToNamesList(e.Location);
            int namesDiff = (mNamesRectWidth - namesPoint.X);

            if (e.Button == MouseButtons.None)
            {
                // Check to see if we're hovering over the frame indicator.
                if (CurrentFrameRect.Contains(timelinePoint))
                {
                    Cursor.Current = Cursors.SizeWE;
                }
                else if (namesDiff >= 0 && namesDiff <= 5)
                {
                    Cursor.Current = Cursors.SizeWE;
                }
            }
            else if (e.Button == MouseButtons.Left)
            {
                // Current Frame Indicator Drag
                if (mDraggingIndicator)
                {
                    MoveFrameIndicator(timelinePoint);
                }
                else if (mDraggingNamesWidth)
                {
                    mNamesRectWidth = namesPoint.X;
                    mNamesRectWidth = Math.Max(mNamesRectWidth, 50);

                    UpdateFrameGridRect();
                    UpdateScrollBars();
                    UpdateRectangles();

                    this.Invalidate();
                }
                // See if we're in the selection rectangle
                else if (mDraggingSelection)
                {
                    Cursor.Current = Cursors.SizeWE;

                    // Figure out which frame we're on
                    int oldFrame = Otter.Utils.LimitRange(0, (mLastMousePos.X - mFrameGridRectangle.Left) / mFrameRect.Width, (int)mAnimation.NumFrames - 1);
                    int newFrame = Otter.Utils.LimitRange(0, (timelinePoint.X - mFrameGridRectangle.Left) / mFrameRect.Width, (int)mAnimation.NumFrames - 1);

                    if (oldFrame != newFrame)
                    {
                        if (mSelectedFrameStart == 0)
                        {
                            mSelectedFrameStart = 1;
                            mSelectedFrameEnd = Math.Max(mSelectedFrameStart, mSelectedFrameEnd);
                        }

                        int diff = (newFrame - oldFrame);

                        if (diff < 0 && (mSelectedFrameStart + diff) <= 0)
                            diff = mSelectedFrameStart - 1;

                        if (diff > 0 && (mSelectedFrameEnd + diff) > (mAnimation.NumFrames - 1))
                            diff = (int)(mAnimation.NumFrames - 1) - mSelectedFrameEnd;

                        List<BaseFrame> frames = new List<BaseFrame>(SelectedFrames);
                        foreach (BaseFrame frame in frames)
                        {
                            frame.Frame = (uint)Otter.Utils.LimitRange(0, frame.Frame + diff, (int)mAnimation.NumFrames - 1);
                        }

                        List<BaseFrame> allFrames = null;
                        if (SelectedChannel == 0)
                        {
                            allFrames = new List<BaseFrame>(mAnimation.MainChannelFrames.OfType<BaseFrame>());
                        }
                        else
                        {
                            TimelineChannel channel = mChannels[mSelectedChannel - 1];
                            allFrames = new List<BaseFrame>(channel.Channel.KeyFrames.OfType<BaseFrame>());
                        }

                        // Adjust the outside keyframes accordingly.
                        foreach (BaseFrame frame in allFrames)
                        {
                            if (!frames.Contains(frame))
                            {
                                // If the keyframe was before the current selection, move it to the end
                                if (frame.Frame < mSelectedFrameStart && frame.Frame >= (mSelectedFrameStart + diff))
                                {
                                    int c = (int)(frame.Frame - mSelectedFrameStart) + 1;
                                    frame.Frame = (uint)(mSelectedFrameEnd + c);
                                }
                                else if (frame.Frame > mSelectedFrameEnd && frame.Frame <= (mSelectedFrameEnd + diff))
                                {
                                    int c = (int)(frame.Frame - mSelectedFrameEnd) - 1;
                                    frame.Frame = (uint)(mSelectedFrameStart + c);
                                }
                            }
                        }


                        mSelectedFrameStart = Otter.Utils.LimitRange(1, mSelectedFrameStart + diff, (int)mAnimation.NumFrames - 1);
                        mSelectedFrameEnd = Otter.Utils.LimitRange(1, mSelectedFrameEnd + diff, (int)mAnimation.NumFrames - 1);

                        UpdateSelectRect();
                        this.Invalidate();
                    }
                }
                else if (mCreatingSelection)
                {
                    int bandIndex = (timelinePoint.Y - mFrameGridRectangle.Top) / mFrameRect.Height;

                    int end = (timelinePoint.X - mFrameGridRectangle.Left) / mFrameRect.Width;
                    end = Otter.Utils.LimitRange(SelectedFrameStart, end, (int)mAnimation.NumFrames - 1);

                    SetSelection(SelectedChannel, SelectedFrameStart, end);

                    UpdateSelectRect();
                    this.Invalidate();
                }
            }

            mLastMousePos = timelinePoint;
        }
        #endregion

        #region Event Handlers : Keys
        /// <summary>
        /// Called when a key has been released
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimelineControl_KeyUp(object sender, KeyEventArgs e)
        {
        }
        #endregion

        #region Event Handlers : Context Menu
        /// <summary>
        /// Called when the context menu is opening
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            // mCreateKeyframeToolStripMenuItem.Enabled = (SelectedChannel > 0) && (SelectedFrameStart != -1) && (SelectedFrameStart == SelectedFrameEnd);
            mClearFramesToolStripMenuItem.Enabled = (SelectedChannel > 0) && (SelectedFrameStart != -1) && (SelectedFrameEnd != -1);
            mCopyFramesToolStripMenuItem.Enabled = (SelectedChannel > 0) && (SelectedFrameStart != -1) && (SelectedFrameEnd != -1);
            mPasteFramesToolStripMenuItem.Enabled = (mCopiedKeyFrames.Count > 0);

            bool bAllowLoop = false;
            bool bAllowClear = false;
            bool bAllowInsertOrTrim = false;

            mLoopFramesToolStripMenuItem.Enabled = false;
            mClearLoopToolStripMenuItem.Enabled = false;

            if ((SelectedChannel == 0) && (SelectedFrameStart != -1) && (SelectedFrameEnd != -1))
            {
                bAllowInsertOrTrim = true;

                // Allow a clear if we're overlapping on repeating frames
                if (mAnimation.RepeatStart != -1 && mAnimation.RepeatEnd != -1 && (mAnimation.RepeatStart <= SelectedFrameEnd || mAnimation.RepeatEnd >= SelectedFrameStart))
                    bAllowClear = true;

                // Allow loop if the selected frames are not completely within the existing loop
                if (mAnimation.RepeatStart == -1 && mAnimation.RepeatEnd == -1 || (mAnimation.RepeatStart > SelectedFrameStart || mAnimation.RepeatEnd < SelectedFrameEnd))
                    bAllowLoop = true;
            }

            mLoopFramesToolStripMenuItem.Enabled = bAllowLoop;
            mClearLoopToolStripMenuItem.Enabled = bAllowClear;
            mInsertFramesToolStripMenuItem.Enabled = bAllowInsertOrTrim;
            mTrimFramesToolStripMenuItem.Enabled = bAllowInsertOrTrim;
        }

        /// <summary>
        /// Called when the user wants to loop the selected frames
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mLoopFramesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedChannel != 0)
                return;

            mAnimation.RepeatStart = (mAnimation.RepeatStart == -1) ? SelectedFrameStart : Math.Min(mAnimation.RepeatStart, SelectedFrameStart);
            mAnimation.RepeatEnd = (mAnimation.RepeatEnd == -1) ? SelectedFrameEnd : Math.Max(mAnimation.RepeatEnd, SelectedFrameEnd);

            this.Invalidate();
        }

        /// <summary>
        /// Clears the selected part of the loop
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mClearLoopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedChannel != 0)
                return;

            int startFrame = mAnimation.RepeatStart;
            int endFrame = mAnimation.RepeatEnd;

            // Trim the end
            if (SelectedFrameStart >= startFrame && SelectedFrameStart <= endFrame)
                endFrame = SelectedFrameStart - 1;

            // Trim the front
            if (SelectedFrameEnd >= startFrame && SelectedFrameEnd <= endFrame)
                startFrame = SelectedFrameEnd + 1;

            // If we've adjust the frame start and end such that they no longer bracket properly,
            // OR the selected frame range encapsulates the entire thing, reset the loop entirely.
            if (startFrame > endFrame || (SelectedFrameStart <= startFrame && SelectedFrameEnd >= endFrame))
            {
                startFrame = -1;
                endFrame = -1;
            }

            mAnimation.RepeatStart = startFrame;
            mAnimation.RepeatEnd = endFrame;

            this.Invalidate();
        }

        /// <summary>
        /// Clears any and all keyframes within the selected range of frames
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mClearFramesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int animIndex = SelectedChannel - 1;
            if (animIndex < 0 || (animIndex >= mChannels.Count))
                return;

            GUIAnimationChannel channel = mChannels[animIndex].Channel;
            CommandManager.AddCommand(new ClearFramesCommand(mAnimation, channel, SelectedFrameStart, SelectedFrameEnd), true);
        }

        /// <summary>
        /// Inserts frames into the current timeline
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mInsertFramesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedFrameStart == -1 || SelectedFrameEnd == -1 || SelectedChannel != 0)
                return;
                
            InsertFramesForm insertFramesForm = new InsertFramesForm();

            if (insertFramesForm.ShowDialog() != DialogResult.OK || insertFramesForm.NumFrames <= 0)
                return;

            CommandManager.AddCommand(new InsertFramesCommand(SelectedFrameStart, insertFramesForm.NumFrames, mAnimation), true);
        }

        /// <summary>
        /// Trims frames from the current timeline
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mTrimFramesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedFrameStart == -1 || SelectedFrameEnd == -1 || SelectedChannel != 0)
                return;

            CommandManager.AddCommand(new TrimFramesCommand(SelectedFrameStart, SelectedFrameEnd, mAnimation), true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mActionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedChannel != 0 || SelectedFrameStart < 0 || SelectedFrameEnd < 0)
                return;

            Otter.Forms.ActionsEditor actionsEditor = new Otter.Forms.ActionsEditor(this.View.Scene);
            MainChannelFrame mainChannelFrame = mAnimation.GetMainChannelFrame((uint)SelectedFrameStart);

            if (mainChannelFrame == null)
            {
                mainChannelFrame = new MainChannelFrame();
                mainChannelFrame.Frame = (uint)SelectedFrameStart;
                mAnimation.MainChannelFrames.Add(mainChannelFrame);
            }

            actionsEditor.Actions = mainChannelFrame.Actions;
            if (actionsEditor.ShowDialog() == DialogResult.OK)
            {
                if (actionsEditor.Actions.Count == 0)
                {
                    mAnimation.MainChannelFrames.Remove(mainChannelFrame);
                }
                else
                {
                    mainChannelFrame.Actions.Clear();
                    mainChannelFrame.Actions.AddRange(actionsEditor.Actions);
                }
            }

            this.Invalidate();
        }
        #endregion

        #region Event Handlers : List Editor
        #endregion

        #region Event Handlers : Control
        /// <summary>
        /// Helper - sets/unsets handlers the 
        /// </summary>
        /// <param name="control"></param>
        /// <param name="set"></param>
        private void SetHandlers(GUIControl control, bool set)
        {
            if (set)
            {
                control.Controls.OnControlAdded += new GUIControlCollection.ControlEventHandler(Controls_OnControlAdded);
                control.Controls.OnControlRemoved += new GUIControlCollection.ControlEventHandler(Controls_OnControlRemoved);
            }
            else
            {
                control.Controls.OnControlAdded -= new GUIControlCollection.ControlEventHandler(Controls_OnControlAdded);
                control.Controls.OnControlRemoved -= new GUIControlCollection.ControlEventHandler(Controls_OnControlRemoved);
            }

            foreach (GUIControl childControl in control.Controls)
                SetHandlers(childControl, set);
        }

        /// <summary>
        /// Called when a control has been added to the sceneView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="control"></param>
        void Controls_OnControlAdded(object sender, GUIControl control)
        {
            List<TimelineChannel> oldChannels = mChannels;
            mChannels = GetChannelList();

            // Remember, '0' is the main channel, so offset by one
            int index = GetVisibleChannelIndex(control, mChannels) + 1;

            // Check to see if we need to move the selected band down
            if (index > 0)
            {
                if (SelectedChannel >= index)
                    SetSelection(SelectedChannel + 1, SelectedFrameStart, SelectedFrameEnd);
            }

            UpdateFrameGridRect();
            UpdateScrollBars();
            UpdateRectangles();

            SetHandlers(control, true);
            this.Invalidate();
        }

        /// <summary>
        /// Called when a control has been removed from the sceneView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="control"></param>
        void Controls_OnControlRemoved(object sender, GUIControl control)
        {
            List<TimelineChannel> oldChannels = mChannels;
            mChannels = GetChannelList();

            // Remember, '0' is the main channel, so offset by one
            int index = GetVisibleChannelIndex(control, oldChannels) + 1;

            // Check to see if we need to move the selected band up
            if (index > 0)
            {
                if (SelectedChannel == index)
                    ResetSelection();
                else if (SelectedChannel > index)
                    SetSelection(SelectedChannel - 1, SelectedFrameStart, SelectedFrameEnd);
            }

            UpdateFrameGridRect();
            UpdateScrollBars();
            UpdateRectangles();

            SetHandlers(control, false);
            this.Invalidate();
        }
        #endregion

        #region Event Handlers : GUI Animation
        /// <summary>
        /// Called when an channel's frame count has changed
        /// </summary>
        /// <param name="sender"></param>
        void GUIAnimation_FrameCountChanged(object sender)
        {
            if (mAnimation == sender)
            {
                UpdateFrameGridRect();
                UpdateScrollBars();
                UpdateRectangles();

                this.Invalidate();
            }
        }

        /// <summary>
        /// Called when an channel has been removed from an channel set
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="channel"></param>
        void GUIAnimation_AnimationChannelRemoved(object sender, GUIAnimationChannel animation)
        {
            mChannels = GetChannelList();
            this.Invalidate(true);
        }

        /// <summary>
        /// Called when an channel has been added to an channel set
        /// </summary> 
        /// <param name="sender"></param>
        /// <param name="channel"></param>
        void GUIAnimation_AnimationChannelAdded(object sender, GUIAnimationChannel animation)
        {
            mChannels = GetChannelList();
            this.Invalidate(true);
        }
        
        /// <summary>
        /// Called when a keyframe has been added
        /// </summary>
        /// <param name="sender"></param>
        void GUIAnimation_KeyframeRemoved(object sender)
        {
            mChannels = GetChannelList();
            this.Invalidate();
        }

        /// <summary>
        /// Called when a keyframe has been removed
        /// </summary>
        /// <param name="sender"></param>
        void GUIAnimation_KeyframeAdded(object sender)
        {
            mChannels = GetChannelList();
            this.Invalidate();
        }
        #endregion

        #region Event Notifiers
        /// <summary>
        /// Notifies that the frame has changed
        /// </summary>
        public void NotifyFrameChanged()
        {
            if (CurrentFrameChanged != null)
                CurrentFrameChanged(this);
        }

        /// <summary>
        /// Notifies that the current channel has changed
        /// </summary>
        public void NotifyAnimationChanged()
        {
            if (AnimationChanged != null)
                AnimationChanged(this);
        }

        /// <summary>
        /// Notifies that the current channel has been updated in some significant way.
        /// </summary>
        public void NotifyAnimationUpdated()
        {
            if (AnimationUpdated != null)
                AnimationUpdated(this);
        }

        /// <summary>
        /// Notifies that the selected control has been changed
        /// </summary>
        public void NotifySelectedControlChanged()
        {
            if (SelectedControlsChanged != null)
                SelectedControlsChanged(this);
        }

        /// <summary>
        /// Notifies that the frame selection has changed
        /// </summary>
        public void NotifyFrameSelectionChanged()
        {
            if (FrameSelectionChanged != null)
                FrameSelectionChanged(this);
        }
        #endregion

        #region Painting
        /// <summary>
        /// Paints the timeline control
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            //----------------------------------------------------------
            // Draw the control names box.
            if (e.ClipRectangle.IntersectsWith(mNamesClipRect))
            {
                e.Graphics.ResetTransform();
                e.Graphics.Clip = new Region(mNamesClipRect);
                e.Graphics.Clip.Intersect(mNamesClipRect);
                e.Graphics.FillRectangle(Brushes.White, mNamesClipRect);

                DrawControlNames(e.Graphics, mNamesRectangle);
            }

            //-----------------------------------------------------------
            // Draw the timeline
            if (e.ClipRectangle.IntersectsWith(mChannelsClipRect))
            {
                e.Graphics.ResetTransform();
                e.Graphics.Clip = new Region(mChannelsClipRect);
                e.Graphics.Clip.Intersect(mChannelsClipRect);

                DrawTimeline(e.Graphics, mChannelsRect);
            }

            //-----------------------------------------------------------
            // Draw the main channel
            if (e.ClipRectangle.IntersectsWith(mMainChannelClipRect))
            {
                e.Graphics.ResetTransform();
                e.Graphics.Clip = new Region(mMainChannelClipRect);
                e.Graphics.Clip.Intersect(mMainChannelClipRect);

                e.Graphics.FillRectangle(Brushes.Wheat, mMainChannelRect);

                int numCols = mMainChannelRect.Width / mFrameRect.Width + 1;
                for (int i = 0; i < numCols; i++)
                {
                    e.Graphics.DrawLine(Pens.DarkGray, mMainChannelRect.Left + (i + 1) * mFrameRect.Width, mMainChannelRect.Top, mMainChannelRect.Left + (i + 1) * mFrameRect.Width, mMainChannelRect.Bottom);

                    // Draw a little triangle indicating that this frame has actions
                    if (mAnimation != null && mAnimation.GetMainChannelFrame((uint)i) != null)
                    {
                        int l = mMainChannelRect.Left + i * mFrameRect.Width;
                        int r = l + mFrameRect.Width;
                        int t = mMainChannelRect.Top;
                        int b = mMainChannelRect.Bottom;

                        e.Graphics.FillPolygon(Brushes.Black, new Point[] { new Point(l, t), new Point(r, t), new Point(l + mFrameRect.Width / 2, t + mFrameRect.Height/3) });
                    }
                }

                if (mAnimation != null && mAnimation.RepeatStart != -1 && mAnimation.RepeatEnd != -1)
                {
                    Point startPt = new Point(mMainChannelRect.Left + mAnimation.RepeatStart * mFrameRect.Width, mMainChannelRect.Top + (mFrameRect.Height / 2));
                    Point endPt = new Point(startPt.X + (mAnimation.RepeatEnd - mAnimation.RepeatStart + 1) * mFrameRect.Width, startPt.Y);

                    e.Graphics.DrawLine(Pens.Black, startPt.X, startPt.Y - 4, startPt.X, startPt.Y + 4);
                    e.Graphics.DrawLine(Pens.Black, startPt.X, startPt.Y, endPt.X, endPt.Y);
                    e.Graphics.DrawLine(Pens.Black, endPt.X, endPt.Y - 4, endPt.X, endPt.Y + 4);
                }
            }

            //-----------------------------------------------------------
            // Draw the ruler
            if (e.ClipRectangle.IntersectsWith(mRulerClipRect))
            {
                e.Graphics.ResetTransform();
                e.Graphics.Clip = new Region(mRulerClipRect);
                e.Graphics.Clip.Intersect(mRulerClipRect);
                DrawRuler(e.Graphics, mRulerRect);

                // Draw the current frame indicator
                Rectangle indicatorRect = CurrentFrameRect;
                indicatorRect.Offset(-mHScrollBar.Value, 0);
                e.Graphics.FillRectangle(Brushes.White, indicatorRect);
                e.Graphics.DrawRectangle(Pens.Black, indicatorRect);

                // Draw the current frame's number right above the frame indicator
                string frameStr = (CurrentFrame + 1).ToString();
                SizeF dim = e.Graphics.MeasureString(frameStr, mFrameNumberFont);

                e.Graphics.DrawString(frameStr,
                                        mFrameNumberFont,
                                        Brushes.Black,
                                        indicatorRect.X + indicatorRect.Width / 2 - dim.Width / 2,
                                        indicatorRect.Y + indicatorRect.Height / 2 - dim.Height / 2);

                // Draw the frame indicator line
                e.Graphics.DrawLine(Pens.Red, indicatorRect.X + indicatorRect.Width / 2, indicatorRect.Y + indicatorRect.Height, indicatorRect.X + indicatorRect.Width / 2, ClientRectangle.Height);
            }

            //----------------------------------------------------------
            // Draw separator borders
            e.Graphics.ResetClip();
            e.Graphics.ResetTransform();
            e.Graphics.DrawRectangle(Pens.Black, mFrameGridRectangle);
            e.Graphics.DrawRectangle(Pens.Black, mNamesClipRect);
            e.Graphics.DrawRectangle(Pens.Black, mChannelsClipRect);
            e.Graphics.DrawLine(Pens.Black, mFrameGridRectangle.X, 0, mFrameGridRectangle.X, this.Height);

            //-----------------------------------------------------------
            // Draw the Selection Box
            Rectangle clipRect = (SelectedChannel == 0) ? mMainChannelClipRect : mChannelsClipRect;
            if (e.ClipRectangle.IntersectsWith(clipRect) && SelectedChannel >= 0)
            {
                e.Graphics.ResetTransform();
                e.Graphics.Clip = new Region(clipRect);
                e.Graphics.Clip.Intersect(clipRect);

                Color selectColor = Color.FromArgb(50, Color.Blue);
                Brush brush = new SolidBrush(selectColor);
                e.Graphics.FillRectangle(brush, mSelectRect);
                e.Graphics.DrawRectangle(Pens.DarkBlue, mSelectRect);
                brush.Dispose();
            }

            // "Gray" out the entire thing if needed
            if (!Enabled || mAnimation == null)
            {
                Color disabledColor = Color.FromArgb(100, Color.Gray);
                Brush brush = new SolidBrush(disabledColor);
                e.Graphics.FillRectangle(brush, 0, 0, ClientRectangle.Width, ClientRectangle.Height);
                brush.Dispose();
            }
        }

        /// <summary>
        /// Updates the clip rectangles
        /// </summary>
        private void UpdateClipRectangles()
        {
            // Set up the clipping rectangles
            Rectangle frameGridClip = new Rectangle(mNamesRectWidth, 20, ClientRectangle.Width - mNamesRectWidth - 16, ClientRectangle.Height - 20 - 16);
            frameGridClip.Height = Math.Max(frameGridClip.Height, 36);
            frameGridClip.Width = Math.Max(frameGridClip.Width, 36);

            mChannelsClipRect = new Rectangle(frameGridClip.X, frameGridClip.Y + mFrameRect.Height, frameGridClip.Width, frameGridClip.Height - mFrameRect.Height);
            mNamesClipRect = new Rectangle(0, frameGridClip.Top + mFrameRect.Height, frameGridClip.Left, mChannelsClipRect.Height);
            mMainChannelClipRect = new Rectangle(frameGridClip.X, frameGridClip.Y, mChannelsClipRect.Width, mFrameRect.Height + 1);
            mRulerClipRect = new Rectangle(frameGridClip.X, 0, frameGridClip.Width, this.Height);
        }

        /// <summary>
        /// Draws the timeline, with all of the channel channels.
        /// </summary>
        private void DrawTimeline(Graphics g, Rectangle rect)
        {
            Brush fadeBrush = new SolidBrush(Color.FromArgb(150, Color.White));
            Brush keyFrameBrush = new SolidBrush(Color.FromArgb(200, Color.DodgerBlue));

            // First fill the entire background with white
            g.FillRectangle(Brushes.White, rect);

            int numRows = rect.Height / mFrameRect.Height;
            int numCols = rect.Width / mFrameRect.Width;

            // Draw the channel frames
            for (int i = 0; i < numRows; i++)
            {
                if (i < mChannels.Count)
                {
                    GUIAnimationChannel channel = mChannels[i].Channel;

                    int x = rect.Left;
                    int y = rect.Top + i * mFrameRect.Height;

                    // Highlight every n'th frame.
                    for (int j = 4; j < mAnimation.NumFrames; j += 5)
                    {
                        g.FillRectangle(Brushes.LightCyan, x + j * mFrameRect.Width, y, mFrameRect.Width, mFrameRect.Height);
                    }

                    if (channel == null)
                        continue;

                    // Draw the channel frames
                    for (int k = 0; k < channel.KeyFrames.Count; k++)
                    {
                        KeyFrame frame = channel.KeyFrames[k];
                        if (frame == null)
                            continue;

                        g.FillRectangle(keyFrameBrush, x + frame.Frame * mFrameRect.Width, y, mFrameRect.Width, mFrameRect.Height);
                    }
                }
            }

            // Now draw the row separators
            for (int i = 0; i < numRows; i++)
            {
                g.DrawLine(Pens.DarkGray, rect.Left, rect.Top + (i + 1) * mFrameRect.Height, rect.Right, rect.Top + (i + 1) * mFrameRect.Height);
            }

            // Draw the column separators
            for (int j = 0; j < numCols; j++)
            {
                g.DrawLine(Pens.DarkGray, rect.Left + (j + 1) * mFrameRect.Width, rect.Top, rect.Left + (j + 1) * mFrameRect.Width, rect.Bottom);
            }

            int numFrames = mAnimation != null ? (int)mAnimation.NumFrames : 0;

            // Draw two transparent white rects over "invalid" frames
            // Note: We offset by 1 to not draw over the valid frame borders.
            g.FillRectangle(fadeBrush, rect.Left, rect.Top + mChannels.Count * mFrameRect.Height + 1, rect.Width, rect.Height);
            g.FillRectangle(fadeBrush, rect.Left + numFrames * mFrameRect.Width + 1, rect.Top, rect.Width, mChannels.Count * mFrameRect.Height + 1);

            keyFrameBrush.Dispose();
            fadeBrush.Dispose();
        }

        /// <summary>
        /// Draws the control names.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rect"></param>
        private void DrawControlNames(Graphics g, Rectangle rect)
        {
            int numRows = rect.Height / mFrameRect.Height + 1;
            for (int i = 0; i < numRows; i++)
            {
                GUIControl control = (i < mChannels.Count) ? mChannels[i].Control : null;
                bool bSelected = (control != null && SelectedControls.Contains(control));

                int x = rect.Left;
                int y = rect.Top + i * mFrameRect.Height;

                if ((i % 2) == 0 || bSelected)
                {
                    Brush brush = bSelected ? Brushes.DarkBlue : Brushes.Azure;

                    if (bSelected && SelectedChannel >= 0)
                        brush = Brushes.LightGray;

                    g.FillRectangle(brush, x, y, rect.Width, mFrameRect.Height);
                }

                if (control != null)
                {
                    Brush brush = (bSelected && SelectedChannel < 0) ? Brushes.White : Brushes.Black;

                    // Draw the control's name
                    string name = (control != null) ? control.Name : "[ERROR]";
                    SizeF dim = g.MeasureString(name, this.Font);
                    g.DrawString(name, this.Font, brush, x + mChannels[i].Indent * 5, y + mFrameRect.Height / 2.0f - dim.Height / 2.0f);
                }

                g.DrawLine(Pens.DarkGray, x, y, x + rect.Width, y);
            }
        }

        /// <summary>
        /// Draws the ruler
        /// </summary>
        /// <param name="e"></param>
        private void DrawRuler(Graphics g, Rectangle rect)
        {
            g.FillRectangle(Brushes.White, rect);
            
            int left = rect.Left;
            int right = rect.Right;

            int frameNum = 0;
            while (left < right)
            {
                g.DrawLine(Pens.DarkGray, left, rect.Bottom, left, rect.Bottom - 3);

                // Draw the frame text if needed
                if (((frameNum + 1) % 5) == 0)
                {
                    string frameStr = (frameNum + 1).ToString();
                    SizeF dim = g.MeasureString(frameStr, mFrameNumberFont);

                    g.DrawString(frameStr, mFrameNumberFont, Brushes.Black, (float)left + mFrameRect.Width / 2.0f - dim.Width / 2.0f,
                                                                            (float)rect.Y + rect.Height / 2.0f - dim.Height / 2.0f - 4);
                }

                left += mFrameRect.Width;
                frameNum++;
            }
        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public TimelineControl()
        {
            InitializeComponent();

            mFrameNumberFont = new Font(this.Font.FontFamily, 7);

            mFrameCountChangedEventHandler = new GUIAnimation.AnimationEventHandler(GUIAnimation_FrameCountChanged);

            mKeyFrameAddedEventHandler = new GUIAnimation.AnimationEventHandler(GUIAnimation_KeyframeAdded);
            mKeyFrameRemovedEventHandler = new GUIAnimation.AnimationEventHandler(GUIAnimation_KeyframeRemoved);

            mAnimationChannelAddedEventHandler = new GUIAnimation.AnimationChannelEventHandler(GUIAnimation_AnimationChannelAdded);
            mAnimationChannelRemovedEventHandler = new GUIAnimation.AnimationChannelEventHandler(GUIAnimation_AnimationChannelRemoved);

            mSelectedControls.OnItemAdded += new NotifyingList<GUIControl>.ListEventHandler(mSelectedControls_OnItemAdded);
            mSelectedControls.OnItemRemoved += new NotifyingList<GUIControl>.ListEventHandler(mSelectedControls_OnItemRemoved);

            Disposed += new EventHandler(TimelineControl_Disposed);
        }

        void mSelectedControls_OnItemAdded(object sender, GUIControl item)
        {
            NotifySelectedControlChanged();
            this.Invalidate();
        }

        void mSelectedControls_OnItemRemoved(object sender, GUIControl item)
        {
            NotifySelectedControlChanged();
            this.Invalidate();
        }

        /// <summary>
        /// Called when the timeline control has been disposed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TimelineControl_Disposed(object sender, EventArgs e)
        {
            mFrameNumberFont.Dispose();
        }

        /// <summary>
        /// Sets the current selection
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void SetSelection(int channel, int start, int end)
        {
            if (channel != mSelectedChannel || start != mSelectedFrameStart || end != mSelectedFrameEnd)
            {
                mSelectedChannel = channel;
                mSelectedFrameStart = start;
                mSelectedFrameEnd = end;

                NotifyFrameSelectionChanged();

                this.Invalidate();
            }
        }

        /// <summary>
        /// Resets the timeline control's selection box
        /// </summary>
        public void ResetSelection()
        {
            if (mSelectedChannel != -1 || mSelectedFrameStart != -1 || mSelectedFrameEnd != -1)
            {
                mSelectedChannel = -1;
                mSelectedFrameStart = -1;
                mSelectedFrameEnd = -1;

                UpdateSelectRect();
                NotifyFrameSelectionChanged();

                this.Invalidate();
            }
        }

        /// <summary>
        /// Moves the frame indicator to the desired location
        /// </summary>
        /// <param name="location"></param>
        private void MoveFrameIndicator(Point location)
        {
            if (mAnimation == null || mAnimation.NumFrames == 0)
                return;

            // Figure out which frame we're on
            CurrentFrame = (UInt32)Otter.Utils.LimitRange(0, (location.X - mFrameGridRectangle.Left) / mFrameRect.Width, (int)mAnimation.NumFrames - 1);
        }

        /// <summary>
        /// Updates the scrollbars to the reflect the state of the current channel
        /// </summary>
        private void UpdateScrollBars()
        {
            if (Animation != null)
            {
                mHScrollBar.Minimum = 0;
                mHScrollBar.Maximum = (int)Animation.NumFrames * mFrameRect.Width;
                mHScrollBar.SmallChange = 1;
                mHScrollBar.LargeChange = mFrameGridRectangle.Width;
                mHScrollBar.Value = Otter.Utils.LimitRange(0, mHScrollBar.Value, Math.Max(0, mHScrollBar.Maximum - mHScrollBar.LargeChange));
                mHScrollBar.Enabled = (mHScrollBar.Maximum - mHScrollBar.Minimum) > mHScrollBar.LargeChange;

                mVScrollBar.Minimum = 0;
                mVScrollBar.Maximum = (int)mChannels.Count * mFrameRect.Height;
                mVScrollBar.SmallChange = 1;
                mVScrollBar.LargeChange = mFrameGridRectangle.Height - mFrameRect.Height;
                mVScrollBar.Value = Otter.Utils.LimitRange(0, mVScrollBar.Value, Math.Max(0, mVScrollBar.Maximum - mVScrollBar.LargeChange));
                mVScrollBar.Enabled = (mVScrollBar.Maximum - mVScrollBar.Minimum) > mVScrollBar.LargeChange;
            }
            else
            {
                mHScrollBar.Enabled = false;
                mHScrollBar.Value = 0;

                mVScrollBar.Enabled = false;
                mVScrollBar.Value = 0;
            }
        }

        /* Updates the frame grid rect
         */
        private void UpdateFrameGridRect()
        {
            mFrameGridRectangle = new Rectangle(mNamesRectWidth, 20, ClientRectangle.Width - mNamesRectWidth - 16, ClientRectangle.Height - 20 - 16);
            if (mFrameGridRectangle.Height < 36)
                mFrameGridRectangle.Height = 36;
            if (mFrameGridRectangle.Width < 116)
                mFrameGridRectangle.Width = 116;
        }

        /// <summary>
        /// Updates the variables rectangles that define regions inside the control
        /// </summary>
        private void UpdateRectangles()
        {
            UpdateFrameGridRect();

            int numRows = Math.Max(mFrameGridRectangle.Height / mFrameRect.Height + 1, mChannels.Count);
            int numCols = Math.Max(mFrameGridRectangle.Width / mFrameRect.Width + 1, mAnimation != null ? (int)mAnimation.NumFrames : 0);

            mChannelsRect = new Rectangle(mFrameGridRectangle.X, mFrameGridRectangle.Y + mFrameRect.Height, numCols * mFrameRect.Width, numRows * mFrameRect.Height);
            mNamesRectangle = new Rectangle(0, mFrameGridRectangle.Y + mFrameRect.Height, mFrameGridRectangle.Left, mChannelsRect.Height);
            mRulerRect = new Rectangle(mFrameGridRectangle.X, mFrameGridRectangle.Y - 20, mChannelsRect.Width, 20);
            mMainChannelRect = new Rectangle(mFrameGridRectangle.X, mFrameGridRectangle.Y, mChannelsRect.Width, mFrameRect.Height + 1);

            UpdateSelectRect();

            mChannelsRect.Offset(-mHScrollBar.Value, -mVScrollBar.Value);
            mNamesRectangle.Offset(0, -mVScrollBar.Value);
            mMainChannelRect.Offset(-mHScrollBar.Value, 0);
            mRulerRect.Offset(-mHScrollBar.Value, 0);

            UpdateClipRectangles();
        }

        /// <summary>
        /// Updates the selection rectangle
        /// </summary>
        private void UpdateSelectRect()
        {
            mSelectRect = Rectangle.Empty;
            if (SelectedChannel != -1 && SelectedFrameStart != -1 && SelectedFrameEnd != -1)
            {
                mSelectRect = new Rectangle(mFrameGridRectangle.Left + SelectedFrameStart * mFrameRect.Width,
                                              mFrameGridRectangle.Top + SelectedChannel * mFrameRect.Height,
                                              mFrameRect.Width * (SelectedFrameEnd - SelectedFrameStart + 1),
                                              mFrameRect.Height);

                mSelectRect.Offset(-mHScrollBar.Value, (SelectedChannel == 0) ? 0 : -mVScrollBar.Value);
            }
        }

        /// <summary>
        /// Invalidate the entire control if resized.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            UpdateFrameGridRect();
            UpdateScrollBars();
            UpdateRectangles();

            this.Invalidate();
        }

        /// <summary>
        /// Called when the control is loading.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimelineControl_Load(object sender, EventArgs e)
        {
            UpdateFrameGridRect();
            UpdateScrollBars();
            UpdateRectangles();
        }

        /// <summary>
        /// Called when the horizontal scrollbar value has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mHScrollBar_ValueChanged(object sender, EventArgs e)
        {
            UpdateRectangles();
            this.Invalidate();
        }

        /// <summary>
        /// Called when the vertical scrollbar value has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mVScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            UpdateRectangles();
            this.Invalidate();
        }

        /// <summary>
        /// Converts the provided point to the correct location on the
        /// timeline
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        private Point ClientToTimeline(Point pt)
        {
            if(pt.Y < (mFrameGridRectangle.Y + mFrameRect.Height))
                return new Point(pt.X + mHScrollBar.Value, pt.Y);

            return new Point(pt.X + mHScrollBar.Value, pt.Y + mVScrollBar.Value);
        }

        /// <summary>
        /// Converts the provided point to the correct location on the
        /// names rectangle
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        private Point ClientToNamesList(Point pt)
        {
            return new Point(pt.X, pt.Y);
        }

        /// <summary>
        /// Adds the selected keyframes into a list to be copied.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mCopyFramesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // is there a frame there?
            if ((SelectedChannel == 0) || (SelectedFrameStart == -1) || (SelectedFrameEnd == -1))
                return;

            // Clear the list of copied key frames
            mCopiedKeyFrames.Clear();

            if (mChannels[SelectedChannel - 1].Channel == null)
                return;
            
            // Grab all of the selected keyframes, clone them and store in a list
            for (int i = SelectedFrameStart; i <= SelectedFrameEnd; i++)
            {
                KeyFrame keyFrame = mChannels[SelectedChannel - 1].Channel.GetKeyFrame(i);

                if (keyFrame != null)
                {
                    KeyFrame item = (KeyFrame)keyFrame.Clone();

                    // Store the relative frame offset from the selected frame start.  We'll
                    // offset the frames once we paste them.
                    item.Frame = (uint)i - (uint)SelectedFrameStart;

                    mCopiedKeyFrames.Add(item);
                }                
            }
        }

        /// <summary>
        /// Pastes the cloned frames starting at the selected frame.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mPasteFramesToolStripMenuItem_Click(object sender, EventArgs e)
        {            
            if (mCopiedKeyFrames.Count == 0)
                return;

            // Make sure we have a valid frame selected
            if ((SelectedChannel == 0) || (SelectedFrameStart == -1))
                return;

            GUIControl selectedControl = mChannels[SelectedChannel - 1].Control;

            if (selectedControl != null)
            {
                foreach (KeyFrame keyFrame in mCopiedKeyFrames)
                {
                    uint newframe = (uint)SelectedFrameStart + keyFrame.Frame;
                    
                    // if (newframe <= SelectedFrameEnd)
                    {
                        KeyFrame newFrame = (KeyFrame)keyFrame.Clone();
                        newFrame.Frame = newframe;
                        mAnimation.CreateKeyFrame(selectedControl, newFrame);
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves the displayable channel list
        /// </summary>
        /// <returns></returns>
        private List<TimelineChannel> GetChannelList()
        {
            return GetChannelList(mView != null ? mView.Controls : null, 0);
        }

        /// <summary>
        /// Populates the list of channels to display.
        /// </summary>
        private List<TimelineChannel> GetChannelList(GUIControlCollection controls, int indent)
        {
            List<TimelineChannel> list = new List<TimelineChannel>();

            if (controls != null && mAnimation != null)
            {
                foreach (GUIControl control in controls)
                {
                    GUIAnimationChannel animChannel = mAnimation.GetAnimationChannel(control);
                    List<TimelineChannel> childList = GetChannelList(control.Controls, indent + 1);

                    if (animChannel != null || ChannelFilter == Filter.All || childList.Count > 0)
                    {
                        TimelineChannel channel = new TimelineChannel();
                        channel.Control = control;
                        channel.Channel = mAnimation.GetAnimationChannel(control);
                        channel.Indent = indent;
                        list.Add(channel);
                    }

                    list.AddRange(childList);
                }
            }

            return list;
        }

        /// <summary>
        /// Retrieves the visible channel index of a control
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        private int GetVisibleChannelIndex(GUIControl control, List<TimelineChannel> channels)
        {
            for (int i = 0; i < channels.Count; i++)
            {
                if (channels[i].Control == control)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
