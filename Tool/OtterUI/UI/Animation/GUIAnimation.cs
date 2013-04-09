using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Drawing.Design;

using Otter.UI.Actions;

namespace Otter.UI.Animation
{
    /// <summary>
    /// Animates a single GUI Control
    /// </summary>
    public class GUIAnimation : ICloneable
    {
        #region Events and Delegates
        public delegate void AnimationEventHandler(object sender);
        public delegate void AnimationChannelEventHandler(object sender, GUIAnimationChannel animation);

        public event AnimationEventHandler FrameChanged = null;
        public event AnimationEventHandler FrameCountChanged = null;

        public event AnimationEventHandler KeyframeAdded = null;
        public event AnimationEventHandler KeyframeRemoved = null;

        public event AnimationChannelEventHandler AnimationChannelAdded = null;
        public event AnimationChannelEventHandler AnimationChannelRemoved = null;
        #endregion

        #region Data
        private string  mName = "";
        private UInt32  mNumFrames = 100;
        private UInt32  mFrame = 0;
        private bool    mRequired = false;

        private int     mRepeatStart = -1;
        private int     mRepeatEnd = -1;

        private List<GUIAnimationChannel> mAnimationChannels = new List<GUIAnimationChannel>();
        private List<MainChannelFrame> mMainChannelFrames = new List<MainChannelFrame>();
        #endregion

        #region Properties
        /// <summary>
        /// Gets / Sets the scene
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public GUIScene Scene { get; set; }

        /// <summary>
        /// Gets / Sets the name of the channel set
        /// </summary>
        public string Name
        {
            get { return mName; }
            set { mName = value; }
        }

        /// <summary>
        /// Gets / Sets the number of frames.
        /// </summary>
        public UInt32 NumFrames
        {
            get { return mNumFrames; }
            set 
            {
                // Number of frames has to be positive
                UInt32 newValue = Math.Max(1, value);

                if (mNumFrames != newValue)
                {
                    mNumFrames = newValue;

                    // Now remove any and all keyframes beyond the last frame
                    foreach (GUIAnimationChannel channel in AnimationChannels)
                    {
                        int cnt = channel.KeyFrames.Count;
                        for (int i = cnt - 1; i >= cnt; i--)
                        {
                            if (channel.KeyFrames[i].Frame >= newValue)
                                channel.KeyFrames.RemoveAt(i);
                        }
                    }

                    NotifyFrameCountChanged();
                }
            }
        }

        /// <summary>
        /// Gets / Sets the channel set's current frame
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public UInt32 Frame
        {
            get { return mFrame; }
            set 
            {
                if (mFrame != value)
                {
                    mFrame = value;
                    UpdateAnimations();

                    NotifyFrameChanged();
                }
            }
        }

        /// <summary>
        /// Gets / Sets the repeat start frame
        /// </summary>
        [Browsable(false)]
		[XmlAttribute]
        public int RepeatStart
        {
            get { return mRepeatStart; }
            set { mRepeatStart = value; }
        }

        /// <summary>
        /// Gets / Sets the repeat end frame
        /// </summary>
        [Browsable(false)]
		[XmlAttribute]
        public int RepeatEnd
        {
            get { return mRepeatEnd; }
            set { mRepeatEnd = value; }
        }


        /// <summary>
        /// Gets / Sets the list of animations channels
        /// </summary>
        [Browsable(false)]
        public List<GUIAnimationChannel> AnimationChannels
        {
            get { return mAnimationChannels; }
            set { mAnimationChannels = value; }
        }

        /// <summary>
        /// Gets / Sets the list of actions for this animation
        /// </summary>
        [Browsable(false)]
        public List<MainChannelFrame> MainChannelFrames
        {
            get { return mMainChannelFrames; }
            set { mMainChannelFrames = value; }
        }
        #endregion

        #region Event Notifiers
        /// <summary>
        /// Notifies that the channel set's current frame has changed
        /// </summary>
        private void NotifyFrameChanged()
        {
            if (FrameChanged != null)
                FrameChanged(this);
        }

        /// <summary>
        /// Notifies that the channel's frame count has changed.
        /// </summary>
        private void NotifyFrameCountChanged()
        {
            if (FrameCountChanged != null)
                FrameCountChanged(this);
        }

        /// <summary>
        /// Notifies that a keyframe has been added
        /// </summary>
        private void NotifyKeyframeAdded()
        {
            if (KeyframeAdded != null)
                KeyframeAdded(this);
        }

        /// <summary>
        /// Notifies that a keyframe has been removed
        /// </summary>
        private void NotifyKeyframeRemoved()
        {
            if (KeyframeRemoved != null)
                KeyframeRemoved(this);
        }

        /// <summary>
        /// Notifies that a new channel channel has been added to this set
        /// </summary>
        private void NotifyAnimationChannelAdded(GUIAnimationChannel animation)
        {
            if (AnimationChannelAdded != null)
                AnimationChannelAdded(this, animation);
        }

        /// <summary>
        /// Notifies that a channel channel has been removed from this set
        /// </summary>
        private void NotifyAnimationChannelRemoved(GUIAnimationChannel animation)
        {
            if (AnimationChannelRemoved != null)
                AnimationChannelRemoved(this, animation);
        }
        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        public GUIAnimation()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        public GUIAnimation(string name, bool bRequired)
        {
            Name = name;
            mRequired = bRequired;
        }

        /// <summary>
        /// Retrieves the channel associated with a particular control
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public GUIAnimationChannel GetAnimationChannel(GUIControl control)
        {
            foreach (GUIAnimationChannel animation in mAnimationChannels)
            {
                if (animation.Control == control)
                    return animation;
            }

            return null;
        }

        /// <summary>
        /// Removes an channel associated with the specified control
        /// </summary>
        /// <param name="control"></param>
        public void RemoveAnimationChannel(GUIControl control)
        {
            int cnt = mAnimationChannels.Count;
            for(int i = 0; i < cnt; i++)
            {
                if (mAnimationChannels[i].Control == control)
                {
                    mAnimationChannels.RemoveAt(i);
                    break;
                }
            }
        }

        /// <summary>
        /// Creates a keyframe at the current frame.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="frame"></param>
        public void CreateKeyFrame(GUIControl control)
        {
            CreateKeyFrame(control, Frame);
        }

        /// <summary>
        /// Creates a keyframe at the specified frame for the control.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="frame"></param>
        public void CreateKeyFrame(GUIControl control, UInt32 frame)
        {
            if (frame >= NumFrames)
                return;

            // If this is the first keyframe for this channel and it's not the first
            // frame, create a first frame automatically.
            if (frame != 0 && GetAnimationChannel(control) == null)
                CreateKeyFrame(control, 0);

            KeyFrame keyFrame = control.CreateKeyFrame();
            keyFrame.Frame = frame;

            CreateKeyFrame(control, keyFrame);
        }

        /// <summary>
        /// Creates a keyframe at the specified frame for the control.
        /// TODO : Move the logic to insert keyframes into a keyframe collection list!
        /// </summary>
        /// <param name="control"></param>
        /// <param name="frame"></param>
        public void CreateKeyFrame(GUIControl control, KeyFrame keyFrame)
        {
            GUIAnimationChannel animation = GetAnimationChannel(control);
            if (animation == null)
            {
                animation = new GUIAnimationChannel();
                animation.Control = control;
                mAnimationChannels.Add(animation);

                NotifyAnimationChannelAdded(animation);
            }

            int numKeyFrames = animation.KeyFrames.Count;
            for (int i = 0; i < numKeyFrames; i++)
            {
                if (animation.KeyFrames[i].Frame == keyFrame.Frame)
                {
                    animation.KeyFrames[i] = keyFrame;
                    return;
                }
            }

            // If we got here, the frame wasn't found, so just add it and sort
            animation.KeyFrames.Add(keyFrame);
            animation.KeyFrames.Sort((a, b) => ((int)a.Frame - (int)b.Frame));

            NotifyKeyframeAdded();
        }

        /// <summary>
        /// Retrieves a main channel frame object by frame
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public MainChannelFrame GetMainChannelFrame(uint frame)
        {
            foreach (MainChannelFrame actionList in mMainChannelFrames)
            {
                if (actionList.Frame == frame)
                    return actionList;
            }

            return null;
        }

        /// <summary>
        /// Called when the current frame has changed.  Update the controls accordingly.
        /// </summary>
        public void UpdateAnimations()
        {
            foreach (GUIAnimationChannel animation in AnimationChannels)
            {
                GUIControl control = animation.Control;
                if (control == null)
                    continue;

                // Find the bracketing keyframes
                KeyFrame startFrame = null;
                KeyFrame endFrame = null;

                foreach (KeyFrame keyFrame in animation.KeyFrames)
                {
                    if (keyFrame.Frame <= Frame)
                        startFrame = keyFrame;
                    else if (keyFrame.Frame > Frame && endFrame == null) 
                        endFrame = keyFrame;
                }

                // Now that we have the start and end frame, figure out the updated specifics
                // for this control
                if (startFrame == null && endFrame == null)
                    continue;

                float factor = 0.0f;
                if (startFrame != null && endFrame != null)
                {
                    factor = (float)(Frame - startFrame.Frame) / (float)(endFrame.Frame - startFrame.Frame);
                    factor = (float)Math.Round(factor, 3);
                    factor = Otter.Interface.Utilities.CalculateEase(factor, (int)startFrame.EaseFunction, startFrame.EaseAmount);
                }

                // Give the factor only a 3 decimal precision.
                factor = (float)Math.Round(factor, 3);
                control.ApplyKeyFrame(startFrame, endFrame, factor);
            }
        }

        /// <summary>
        /// Clones this animation
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            GUIAnimation anim = new GUIAnimation();

            anim.mName = this.Name;
            anim.mNumFrames = this.mNumFrames;
            anim.mFrame = this.mFrame;
            anim.mRequired = this.mRequired;

            anim.mRepeatStart = this.mRepeatStart;
            anim.mRepeatEnd = this.mRepeatEnd;

            foreach(GUIAnimationChannel channel in this.mAnimationChannels)
                anim.mAnimationChannels.Add((GUIAnimationChannel)channel.Clone());

            foreach(MainChannelFrame frame in this.mMainChannelFrames)
                anim.mMainChannelFrames.Add((MainChannelFrame)frame.Clone());

            return anim;
        }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }

        private void BuildSortedAnimations(GUIControlCollection controls, List<GUIAnimationChannel> channels)
        {
            foreach (GUIControl control in controls)
            {
                foreach (GUIAnimationChannel channel in AnimationChannels)
                {
                    if (channel.Control == control)
                    {
                        channels.Add(channel);
                        break;
                    }
                }

                BuildSortedAnimations(control.Controls, channels);
            }
        }

        /// <summary>
        /// Returns a sorted list of channels where children follow parents
        /// </summary>
        /// <param name="controls"></param>
        /// <returns></returns>
        public List<GUIAnimationChannel> GetSortedChannels(GUIControlCollection controls)
        {
            List<GUIAnimationChannel> channels = new List<GUIAnimationChannel>();

            if (controls.Count == 0 || AnimationChannels.Count == 0)
                return channels;

            BuildSortedAnimations(controls, channels);
            return channels;
        }
    }
}
