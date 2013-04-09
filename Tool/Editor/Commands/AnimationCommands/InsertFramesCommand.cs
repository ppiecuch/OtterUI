
using System;
using System.Collections.Generic;
using System.Text;

using Otter.Commands;
using Otter.UI.Animation;
using Otter.UI.Actions;

namespace Otter.Editor.Commands.AnimationCommands
{
    class InsertFramesCommand: Command
    {
        #region Data
        private GUIAnimation mAnimation = null;
        private int mPosition = 0;
        private int mCount = 0;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="trimStart"></param>
        /// <param name="trimEnd"></param>
        /// <param name="channel"></param>
        public InsertFramesCommand(int position, int count, GUIAnimation animation)
        {
            mAnimation = animation;
            mPosition = position;
            mCount = count;
        }

        /// <summary>
        /// Executes the command
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            if (mCount == 0)
                return false;

            mAnimation.NumFrames += (uint)mCount;

            // Shift the channel keyframes
            foreach (GUIAnimationChannel channel in mAnimation.AnimationChannels)
            {
                foreach (KeyFrame keyFrame in channel.KeyFrames)
                {
                    if (keyFrame.Frame != 0 && keyFrame.Frame >= mPosition)
                        keyFrame.Frame += (uint)mCount;
                }
            }

            // Shift the action lists
            foreach (MainChannelFrame mainChannelFrame in mAnimation.MainChannelFrames)
            {
                if (mainChannelFrame.Frame >= mPosition)
                    mainChannelFrame.Frame += (uint)mCount;
            }

            return true;
        }

        /// <summary>
        /// Undoes the command
        /// </summary>
        /// <returns></returns>
        public override bool Undo()
        {
            if (mCount == 0)
                return false;

            mAnimation.NumFrames -= (uint)mCount;
            foreach (GUIAnimationChannel channel in mAnimation.AnimationChannels)
            {
                foreach (KeyFrame keyFrame in channel.KeyFrames)
                {
                    if (keyFrame.Frame != 0 && keyFrame.Frame >= mPosition)
                        keyFrame.Frame -= (uint)mCount;
                }
            } 

            foreach (MainChannelFrame mainChannelFrame in mAnimation.MainChannelFrames)
            {
                if (mainChannelFrame.Frame >= mPosition)
                    mainChannelFrame.Frame -= (uint)mCount;
            }

            return true;
        }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Insert " + mCount + " at frame " + mPosition;
        }
    }
}
