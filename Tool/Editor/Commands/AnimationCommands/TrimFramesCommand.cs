using System;
using System.Collections.Generic;
using System.Text;

using Otter.Commands;
using Otter.UI.Animation;
using Otter.UI.Actions;

namespace Otter.Editor.Commands.AnimationCommands
{
    class TrimFramesCommand : Command
    {
        #region Data
        private GUIAnimation mAnimation = null;
        private int mTrimStart = 0;
        private int mTrimEnd = 0;

        private int mOrigRepeatStart = -1;
        private int mOrigRepeatEnd = -1;
        private List<MainChannelFrame> mRemovedMainChannelFrames = new List<MainChannelFrame>();
        private List<List<KeyFrame>> mRemovedKeyFrames = new List<List<KeyFrame>>();
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="trimStart"></param>
        /// <param name="trimEnd"></param>
        /// <param name="channel"></param>
        public TrimFramesCommand(int trimStart, int trimEnd, GUIAnimation animation)
        {
            mAnimation = animation;
            mTrimStart = trimStart;
            mTrimEnd = trimEnd;

            mOrigRepeatStart = animation.RepeatStart;
            mOrigRepeatEnd = animation.RepeatEnd;
        }

        /// <summary>
        /// Executes the command
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            int numFrames = (mTrimEnd - mTrimStart) + 1;
            if (numFrames <= 0)
                return false;

            mRemovedMainChannelFrames.Clear();
            mRemovedKeyFrames.Clear();

            Predicate<KeyFrame> kfPredicate = (frame) => (frame.Frame != 0 && frame.Frame >= mTrimStart && frame.Frame <= mTrimEnd);
            Predicate<MainChannelFrame> mainPredicate = (frame) => (frame.Frame != 0 && frame.Frame >= mTrimStart && frame.Frame <= mTrimEnd);

            mAnimation.NumFrames -= (uint)numFrames;

            // Remove the keyframes
            for(int i = 0; i < mAnimation.AnimationChannels.Count; i++)
            {
                GUIAnimationChannel channel = mAnimation.AnimationChannels[i];
                mRemovedKeyFrames.Add(channel.KeyFrames.FindAll(kfPredicate));

                // Remove unneeded keyframes
                channel.KeyFrames.RemoveAll(kfPredicate);

                foreach (KeyFrame keyFrame in channel.KeyFrames)
                {
                    if (keyFrame.Frame != 0 && keyFrame.Frame >= mTrimStart)
                        keyFrame.Frame -= (uint)numFrames;
                }
            }

            // Remove the action lists
            mRemovedMainChannelFrames.AddRange(mAnimation.MainChannelFrames.FindAll(mainPredicate));
            mAnimation.MainChannelFrames.RemoveAll(mainPredicate);
            foreach (MainChannelFrame actionList in mAnimation.MainChannelFrames)
            {
                if (actionList.Frame >= mTrimStart)
                    actionList.Frame -= (uint)numFrames;
            }

            // Now adjust the loop if necessary
            if (mAnimation.RepeatStart < mAnimation.RepeatEnd)
            {
                // Adjust the start / end frames if necessary
                if (mTrimEnd <= mAnimation.RepeatEnd)
                {
                    mAnimation.RepeatEnd -= numFrames;
                }
                else if (mTrimStart <= mAnimation.RepeatEnd)
                {
                    mAnimation.RepeatEnd = mTrimStart - 1;
                }

                if (mTrimEnd < mAnimation.RepeatStart)
                {
                    mAnimation.RepeatStart -= numFrames;
                }
                else if (mTrimStart <= mAnimation.RepeatStart)
                {
                    mAnimation.RepeatStart = mTrimStart;
                }

                if (mAnimation.RepeatStart > mAnimation.RepeatEnd)
                    mAnimation.RepeatStart = mAnimation.RepeatEnd = -1;
            }

            return true;
        }

        /// <summary>
        /// Undoes the command
        /// </summary>
        /// <returns></returns>
        public override bool Undo()
        {
            int numFrames = (mTrimEnd - mTrimStart) + 1;
            if (numFrames <= 0)
                return false;

            mAnimation.NumFrames += (uint)numFrames;
            mAnimation.RepeatStart = mOrigRepeatStart;
            mAnimation.RepeatEnd = mOrigRepeatEnd;

            // Add the trimmed keyframes back in
            for (int i = 0; i < mAnimation.AnimationChannels.Count; i++)
            {
                GUIAnimationChannel channel = mAnimation.AnimationChannels[i];

                foreach (KeyFrame keyFrame in channel.KeyFrames)
                {
                    if (keyFrame.Frame != 0 && keyFrame.Frame >= mTrimStart)
                        keyFrame.Frame += (uint)numFrames;
                }
                
                channel.KeyFrames.AddRange(mRemovedKeyFrames[i]);
                channel.KeyFrames.Sort((a, b) => ((int)a.Frame - (int)b.Frame));
            }

            // Add the trimmed actionlist back in
            foreach (MainChannelFrame actionList in mAnimation.MainChannelFrames)
            {
                if (actionList.Frame >= mTrimStart)
                    actionList.Frame += (uint)numFrames;
            }

            mAnimation.MainChannelFrames.AddRange(mRemovedMainChannelFrames);
            mAnimation.MainChannelFrames.Sort((a, b) => ((int)a.Frame - (int)b.Frame));

            mRemovedMainChannelFrames.Clear();
            mRemovedKeyFrames.Clear();

            return true;
        }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Trim Frames " + mTrimStart + " - " + mTrimEnd;
        }
    }
}
