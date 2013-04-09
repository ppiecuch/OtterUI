
using System;
using System.Collections.Generic;
using System.Text;

using Otter.Commands;
using Otter.UI.Animation;

namespace Otter.Editor.Commands.AnimationCommands
{
    /// <summary>
    /// Command to clear a set of keyframes from an animation
    /// </summary>
    class ClearFramesCommand : Command
    {
        #region Data
        private GUIAnimation mAnimation = null;
        private GUIAnimationChannel mAnimationChannel = null;
        private int mStart = -1;
        private int mEnd = -1;
        private List<KeyFrame> mKeyFrames = null;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="frames"></param>
        /// <param name="animation"></param>
        public ClearFramesCommand(GUIAnimation animation, GUIAnimationChannel animationChannel, int start, int end)
        {
            mAnimation = animation;
            mAnimationChannel = animationChannel;
            mStart = start;
            mEnd = end;
            mKeyFrames = new List<KeyFrame>();
        }

        /// <summary>
        /// Executes the command : Removes the set of keyframes from the animation
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            if (mStart == -1 || mEnd == -1 || !mAnimation.AnimationChannels.Contains(mAnimationChannel))
                return false;

            mKeyFrames.Clear();
            foreach (KeyFrame frame in mAnimationChannel.KeyFrames)
            {
                if (frame.Frame >= mStart && frame.Frame <= mEnd && frame.Frame >= 0)
                    mKeyFrames.Add(frame);
            }

            if (mKeyFrames.Count == mAnimationChannel.KeyFrames.Count)
            {
                mAnimation.AnimationChannels.Remove(mAnimationChannel);
            }
            else
            {
                foreach (KeyFrame frame in mKeyFrames)
                {
                    // Do not remove frame 0 if we're not removing ALL of the frames
                    if (frame.Frame == 0)
                        continue;

                    mAnimationChannel.KeyFrames.Remove(frame);
                }
            }

            return true;
        }

        /// <summary>
        /// Undoes the command - re-adds the channel if it was removed, otherwise adds
        /// the removed keyframes.
        /// </summary>
        /// <returns></returns>
        public override bool Undo()
        {
            if (!mAnimation.AnimationChannels.Contains(mAnimationChannel))
            {
                mAnimation.AnimationChannels.Add(mAnimationChannel);
            }
            else
            {
                mAnimationChannel.KeyFrames.AddRange(mKeyFrames);
                mAnimationChannel.KeyFrames.Sort((a, b) => ((int)a.Frame - (int)b.Frame));
            }

            return true;
        }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Clear KeyFrames";
        }
    }
}
