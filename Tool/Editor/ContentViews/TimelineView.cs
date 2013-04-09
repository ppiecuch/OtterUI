using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using Otter.Commands;
using Otter.UI;
using Otter.UI.Animation;
using Otter.Editor.CustomControls;
using Otter.Editor.Forms;
using Otter.Containers;

using WeifenLuo.WinFormsUI.Docking;

namespace Otter.Editor.ContentViews
{
    public partial class TimelineView : DockContent
    {
        #region Events and Delegates
        public delegate void TimelineViewEventHandler(object sender);

        public event TimelineViewEventHandler AnimationUpdated = null;
        public event TimelineViewEventHandler SelectedControlChanged = null;
        public event TimelineViewEventHandler FrameSelectionChanged = null;
        #endregion

        #region Event Notifiers
        /// <summary>
        /// Notifies that the current channel has been updated.
        /// </summary>
        private void NotifyAnimationUpdated()
        {
            if(AnimationUpdated != null)
                AnimationUpdated(this);
        }

        /// <summary>
        /// Notifies that the currently selected control has changed
        /// </summary>
        private void NotifySelectedControlChanged()
        {
            if (SelectedControlChanged != null)
                SelectedControlChanged(this);
        }

        /// <summary>
        /// Notifies that the frame selection has changed
        /// </summary>
        private void NotifyFrameSelectionChanged()
        {
            if (FrameSelectionChanged != null)
                FrameSelectionChanged(this);
        }
        #endregion

        #region Data
        private GUIView mView = null;

        private GUIView.AnimationSetEventHandler mAnimationSetAddedEventHandler = null;
        private GUIView.AnimationSetEventHandler mAnimationSetRemovedEventHandler = null;
        private Thread mPlayThread = null;
        private uint mPlayAnimationFrame = 0;
        private bool mKeyOff = false;
        #endregion

        #region Properties
        /// <summary>
        /// Gets / Sets the sceneView associated with the timeline
        /// </summary>
        public GUIView View
        {
            get { return mView; }
            set 
            {
                if (mView != value)
                {
                    // Kill the thread if it's playing
                    if (mPlayThread != null)
                    {
                        mPlayThread.Abort();
                        mPlayThread = null;
                    }

                    if (mView != null)
                    {
                        mView.AnimationSetAdded -= mAnimationSetAddedEventHandler;
                        mView.AnimationSetRemoved -= mAnimationSetRemovedEventHandler;
                    }

                    mView = value;
                    mTimelineControl.View = value;

                    RepopulateControls();

                    if (mView != null)
                    {
                        mView.AnimationSetAdded += mAnimationSetAddedEventHandler;
                        mView.AnimationSetRemoved += mAnimationSetRemovedEventHandler;

                        mAnimationsToolStripComboBox.SelectedItem = mView.CurrentAnimation != null ? mView.CurrentAnimation : mView.Animations["OnActivate"];
                    }
                }
            }
        }

        /// <summary>
        /// Gets / Sets the internally selected control
        /// </summary>
        public NotifyingList<GUIControl> SelectedControls
        {
            get { return mTimelineControl.SelectedControls; }
            set { mTimelineControl.SelectedControls = value; }
        }

        /// <summary>
        /// Retrieves a list of selected keyframes
        /// </summary>
        public List<BaseFrame> SelectedKeyFrames
        {
            get
            {
                return mTimelineControl.SelectedFrames;
            }
        }

        /// <summary>
        /// Gets / Sets the command manager for the sceneView
        /// </summary>
        public CommandManager CommandManager
        {
            get { return mTimelineControl.CommandManager; }
            set { mTimelineControl.CommandManager = value; }
        }
        #endregion

        #region Event Handlers : View
        /// <summary>
        /// Called when an channel set has been added to the sceneView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="set"></param>
        private void View_AnimationAdded(object sender, GUIAnimation animation)
        {
            mAnimationsToolStripComboBox.Items.Insert(mAnimationsToolStripComboBox.Items.Count - 1, animation);
            this.Invalidate(true);
        }

        /// <summary>
        /// Called when an channel set has been removed from the sceneView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="set"></param>
        private void View_AnimationRemoved(object sender, GUIAnimation animation)
        {
            mAnimationsToolStripComboBox.Items.Remove(animation);
            this.Invalidate(true);
        }
        #endregion

        #region Event Handlers : Timeline View
        /// <summary>
        /// Called when the Timeline View Form has been loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimelineView_Load(object sender, EventArgs e)
        {
            mFilterToolStripComboBox.SelectedIndex = (int)TimelineControl.Filter.Animated;
        }
        #endregion

        #region Event Handlers : Filter Combo Box
        /// <summary>
        /// Called when the filter toolstrip combo box's selected index has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mFilterToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            mTimelineControl.ChannelFilter = (TimelineControl.Filter)mFilterToolStripComboBox.SelectedIndex;
        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public TimelineView()
        {
            InitializeComponent();
            RepopulateControls();

            mTimelineControl.CurrentFrameChanged += new Otter.Editor.CustomControls.TimelineControl.TimelineControlEventHandler(mTimelineControl_CurrentFrameChanged);
            mTimelineControl.AnimationUpdated += new Otter.Editor.CustomControls.TimelineControl.TimelineControlEventHandler(mTimelineControl_AnimationSetUpdated);
            mTimelineControl.SelectedControlsChanged += new TimelineControl.TimelineControlEventHandler(mTimelineControl_SelectedControlsChanged);
            mTimelineControl.FrameSelectionChanged += new TimelineControl.TimelineControlEventHandler(mTimelineControl_FrameSelectionChanged);

            mAnimationSetAddedEventHandler = new GUIView.AnimationSetEventHandler(View_AnimationAdded);
            mAnimationSetRemovedEventHandler = new GUIView.AnimationSetEventHandler(View_AnimationRemoved);

            this.HideOnClose = true;
        }

        /// <summary>
        /// Repopulates the controls and sets everything to defaults
        /// based on the current sceneView.
        /// </summary>
        private void RepopulateControls()
        {
            if (View == null)
            {
                mToolstrip.Enabled = false;
                return;
            }

            mToolstrip.Enabled = true;
            mManageAnimationsToolStripButton.Enabled = true;
            mAnimationsToolStripComboBox.Enabled = true;
            mTimelineControl.Enabled = true;
            mPauseButton.Enabled = true;
            mKeyOffButton.Enabled = false;
            mAutoKeyToolstripButton.Checked = Otter.Editor.Properties.Settings.Default.AutoKeyFrame;

            PopulateAnimations(View.Animations);
        }

        /// <summary>
        /// Populates the combo box with the list of animations
        /// </summary>
        /// <param name="list"></param>
        private void PopulateAnimations(GUIAnimationCollection animationCollection)
        {
            mAnimationsToolStripComboBox.Items.Clear();

            // Add the channel sets to the dropdown
            foreach (GUIAnimation animation in animationCollection)
                mAnimationsToolStripComboBox.Items.Add(animation);
        }

        /// <summary>
        /// Called when the current frame in the timeline control has changed.
        /// Modify all the control positions.
        /// 
        /// TODO : Move the functionality out of this class.
        /// </summary>
        /// <param name="sender"></param>
        void mTimelineControl_CurrentFrameChanged(object sender)
        {
            NotifyAnimationUpdated();
        }

        /// <summary>
        /// Called when the timeline control has updated the channel in some significant way
        /// </summary>
        /// <param name="sender"></param>
        void mTimelineControl_AnimationSetUpdated(object sender)
        {
            NotifyAnimationUpdated();
        }

        /// <summary>
        /// Called when the timeline control's selected control has been changed
        /// </summary>
        /// <param name="sender"></param>
        void mTimelineControl_SelectedControlsChanged(object sender)
        {
            NotifySelectedControlChanged();
        }

        /// <summary>
        /// Called whenever the frame selection has changed
        /// </summary>
        /// <param name="sender"></param>
        void mTimelineControl_FrameSelectionChanged(object sender)
        {
            NotifyFrameSelectionChanged();
        }

        /// <summary>
        /// Opens the Animation List Editor.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mManageAnimationsToolStripButton_Click(object sender, EventArgs e)
        {
            ListEditor<GUIAnimation> animSetEditor = new ListEditor<GUIAnimation>(View.Animations.AnimationList);
            animSetEditor.AddingItem += new ListEditor<GUIAnimation>.ListEditorEventHandler(animSetEditor_AddingItem);
            animSetEditor.RemovingItem += new ListEditor<GUIAnimation>.ListEditorEventHandler(animSetEditor_RemovingItem);
            animSetEditor.Text = "Animations";
            animSetEditor.ShowDialog();

            GUIAnimation animation = View.CurrentAnimation;
            View.Animations = new GUIAnimationCollection(animSetEditor.List.ToArray());
            PopulateAnimations(View.Animations);

            // We've replaced the entire channel list.  See if the timeline control's current channel is still
            // present.  If so, nothing needs to be done.  But if it was removed, select the "OnActivate" channel
            if (!View.Animations.Contains(mTimelineControl.Animation))
            {
                mTimelineControl.Animation = View.Animations["OnActivate"];
                mAnimationsToolStripComboBox.SelectedItem = mTimelineControl.Animation;
            }
            else
            {
                mTimelineControl.Animation = animation;
                mAnimationsToolStripComboBox.SelectedItem = animation;
            }
        }

        /// <summary>
        /// User changed animations.  The last index is the "Edit.." item.  If that's selected,
        /// bring up the channel set editor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mAnimationsToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Pause();

            mTimelineControl.Animation = mAnimationsToolStripComboBox.SelectedItem as GUIAnimation;
            mView.CurrentAnimation = mTimelineControl.Animation;
            mView.CurrentAnimation.UpdateAnimations();
            NotifyAnimationUpdated();
        }

        /// <summary>
        /// Called when the channel editing dialog is adding a new item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void animSetEditor_AddingItem(object sender, ListEditor<GUIAnimation>.ListEditorEventArgs e)
        {
            ListEditor<GUIAnimation> editor = sender as ListEditor<GUIAnimation>;
            GUIAnimation animation = e.Object as GUIAnimation;

            if (editor != null && animation != null)
            {
                animation.Name = GetUniqueName("Animation", editor.List);
                animation.NumFrames = 100;
            }
            else
            {
                e.CancelAction = true;
            }
        }

        /// <summary>
        /// Called when the user hit the play button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mPlayButton_Click(object sender, EventArgs e)
        {
            Play();
        }

        /// <summary>
        /// Called when the user hit the pause button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mPauseButton_Click(object sender, EventArgs e)
        {
            Pause();
        }

        /// <summary>
        /// Signals that we need to key off out of the loop
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mKeyOffButton_Click(object sender, EventArgs e)
        {
            mKeyOff = true;
        }

        /// <summary>
        /// Toggles the global Auto-Key flag.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mAutoKeyToolstripButton_Click(object sender, EventArgs e)
        {
            Otter.Editor.Properties.Settings.Default.AutoKeyFrame = mAutoKeyToolstripButton.Checked;
            Otter.Editor.Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Starts animation playback.
        /// </summary>
        private void Play()
        {
            if (mPlayThread == null)
            {
                mKeyOffButton.Enabled = (mView.CurrentAnimation.RepeatStart != -1);
                mPlayButton.Enabled = false;
                mPauseButton.Enabled = true;
                mManageAnimationsToolStripButton.Enabled = false;
                mAnimationsToolStripComboBox.Enabled = false;
                mFilterToolStripComboBox.Enabled = false;

                mPlayThread = new Thread(new ParameterizedThreadStart(PlayAnimationThread));
                mPlayThread.Start(null);
            }
        }

        /// <summary>
        /// Pauses playback.  Safe to call even if no playback is active.
        /// </summary>
        private void Pause()
        {
            if (mPlayThread != null)
            {
                mPlayThread.Abort();
                mPlayThread = null;
            }

            mKeyOffButton.Enabled = false;
            mPlayButton.Enabled = true;
            mPauseButton.Enabled = false;
            mManageAnimationsToolStripButton.Enabled = true;
            mAnimationsToolStripComboBox.Enabled = true;
            mFilterToolStripComboBox.Enabled = true;
        }

        /// <summary>
        /// Called whenever the play timer ticks.  Progresses our channel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PlayAnimationThread(object data)
        {
            int timestamp = System.Environment.TickCount;
            bool bInLoop = false;
            mKeyOff = false;

            float frameLength = 1000.0f / 60.0f;

            float currFrame = 0.0f;
            float prevFrame = currFrame;

            while (mView != null && mView.CurrentAnimation != null)
            {
                uint delta = 0;
                while((delta = (uint)(System.Environment.TickCount - timestamp)) < frameLength);
                timestamp = System.Environment.TickCount;

                prevFrame = currFrame;
                currFrame += delta / frameLength;

                if (bInLoop == false && 
                    mView.CurrentAnimation.RepeatStart != -1 && 
                    prevFrame < mView.CurrentAnimation.RepeatStart && 
                    currFrame >= mView.CurrentAnimation.RepeatStart)
                {
                    bInLoop = true;
                    this.BeginInvoke(new MethodInvoker(EnableKeyOff));
                }
                else if (bInLoop && mView.CurrentAnimation.RepeatStart == -1)
                {
                    bInLoop = false;
                    this.BeginInvoke(new MethodInvoker(DisableKeyOff));
                }

                if (bInLoop)
                {
                    if (currFrame > mView.CurrentAnimation.RepeatEnd)
                    {
                        if (mKeyOff)
                        {
                            bInLoop = false;
                            mKeyOff = false;
                            this.BeginInvoke(new MethodInvoker(DisableKeyOff));
                        }
                        else
                        {
                            currFrame = mView.CurrentAnimation.RepeatStart;
                        }
                    }
                }
                else
                {
                    if (currFrame > mView.CurrentAnimation.NumFrames)
                        currFrame = 0.0f;
                }

                mPlayAnimationFrame = (uint)currFrame;

                this.BeginInvoke(new MethodInvoker(UpdateAnimation));
            }
        }

        /// <summary>
        /// Updates the channel by setting the frame index according to the amount 
        /// of time that has passed.
        /// </summary>
        void UpdateAnimation()
        {
            mTimelineControl.CurrentFrame = mPlayAnimationFrame;
        }

        void EnableKeyOff()
        {
            mKeyOffButton.Enabled = true;
        }

        void DisableKeyOff()
        {
            mKeyOffButton.Enabled = false;
        }

        /// <summary>
        /// Called when the channel editing dialog is removing an item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void animSetEditor_RemovingItem(object sender, ListEditor<GUIAnimation>.ListEditorEventArgs e)
        {
            GUIAnimation animation = e.Object as GUIAnimation;

            if (animation.Name == "OnActivate" || animation.Name == "OnDeactivate")
                e.CancelAction = true;
        }

        /// <summary>
        /// Returns a unuque GUIAnimation name
        /// </summary>
        /// <param name="baseName"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        private string GetUniqueName(string baseName, List<GUIAnimation> list)
        {
            int cnt = 1;
            foreach (GUIAnimation animation in list)
            {
                if (animation.Name == (baseName + " " + cnt))
                    cnt++;
            }

            return baseName + " " + cnt;
        }

        /// <summary>
        /// Refreshes the timeline, causing it to immediately render
        /// </summary>
        public void RefreshTimeline()
        {
            mTimelineControl.Refresh();
        }
    }
}
