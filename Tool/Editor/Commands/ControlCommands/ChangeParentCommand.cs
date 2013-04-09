using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using Otter.UI;
using Otter.Interface;
using Otter.UI.Animation;

namespace Otter.Editor.Commands.ControlCommands
{
    class ChannelState
    {
        public GUIAnimationChannel mChannel = null;
        public List<ControlLayout> mLayouts = new List<ControlLayout>();
    }

    class ControlState
    {
        public GUIControl mControl = null;
        public GUIControl mParent = null;
        public int mIndex = -1;

        public PointF mCenter = PointF.Empty;
        public float mRotation = 0.0f;
        public PointF mLocation = PointF.Empty;

        public List<ChannelState> mChannelStates = new List<ChannelState>();
    }

    /// <summary>
    /// Changes the parent control of a set of controls
    /// </summary>
    public class ChangeParentCommand : Otter.Commands.Command
    {
        #region Data
        private List<GUIControl> mControls = new List<GUIControl>();
        private List<ControlState> mControlStates = new List<ControlState>();

        private GUIControl mNewParent = null;
        private int mIndex = -1;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="controls"></param>
        /// <param name="newParent"></param>
        public ChangeParentCommand(List<GUIControl> controls, GUIControl newParent, int index)
        {
            foreach (GUIControl control in controls)
            {
                ControlState controlState = new ControlState();
                controlState.mControl = control;
                controlState.mParent = control.Parent;
                controlState.mIndex = control.Parent.Controls.IndexOf(control);
                controlState.mCenter = control.Center;
                controlState.mRotation = control.Rotation;
                controlState.mLocation = control.Location;

                // Store the original frame layouts and things
                foreach (GUIAnimation animation in control.ParentView.Animations)
                {
                    foreach (GUIAnimationChannel channel in animation.AnimationChannels)
                    {
                        if (channel.Control != control)
                            continue;

                        ChannelState channelState = new ChannelState();
                        channelState.mChannel = channel;

                        foreach (KeyFrame keyFrame in channel.KeyFrames)
                        {
                            channelState.mLayouts.Add((ControlLayout)keyFrame.Layout.Clone());
                        }

                        controlState.mChannelStates.Add(channelState);
                    }
                }

                mControlStates.Add(controlState);
            }

            mControls.AddRange(controls);
            mNewParent = newParent;
            mIndex = index;
        }

        /// <summary>
        /// Executes the command.  Sets the control's new parent to the one provided in the constructor
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            if (mNewParent == null)
                return false;

            int index = mIndex;
            foreach (GUIControl control in mControls)
            {
                SetParent(control, mNewParent, index);

                if (index != 0)
                    index++;
            }

            return true;
        }

        /// <summary>
        /// Undoes the set-parent command.  Instead of doing the opposite of the
        /// Execute function, we simply restore everything to their original values
        /// </summary>
        /// <returns></returns>
        public override bool Undo()
        {
            if (mNewParent == null)
                return false;

            for(int i = mControlStates.Count - 1; i >= 0; i--)
            {
                GUIControl control = mControlStates[i].mControl;
                GUIControl origParent = mControlStates[i].mParent;
                int origIndex = mControlStates[i].mIndex;

                GUIView view = (origParent is GUIView) ? origParent as GUIView : origParent.ParentView;

                if (view == control.ParentView)
                {
                    view.SetNewParent(control, origParent, origIndex);

                    foreach (ChannelState channelState in mControlStates[i].mChannelStates)
                    {
                        for(int j = 0; j < channelState.mChannel.KeyFrames.Count; j++)
                        {
                            channelState.mChannel.KeyFrames[j].Layout = (ControlLayout)channelState.mLayouts[j].Clone();
                        }
                    }
                }
                else
                {
                    control.ParentView.RemoveControl(control);
                    view.AddControl(origParent, control);
                }

                control.Center = mControlStates[i].mCenter;
                control.Rotation = mControlStates[i].mRotation;
                control.Location = mControlStates[i].mLocation;
            }

            return true;
        }

        /// <summary>
        /// Sets the control's new parent.  This function will also fix up any
        /// other animations that references the control.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="newParent"></param>
        private static void SetParent(GUIControl control, GUIControl newParent, int index)
        {
            GUIView view = (newParent is GUIView) ? newParent as GUIView : newParent.ParentView;
            Otter.Interface.Matrix invTransform = newParent.FullTransformInv;

            PointF center = PointF.Empty;
            float rotation = 0.0f;
            PointF location = PointF.Empty;

            Matrix oldTransformInv = control.TransformInv;
            Matrix newTransform = control.FullTransform * invTransform;
            Matrix toLocal = oldTransformInv * newTransform;

            GetComponents(control, ref newTransform, out center, out rotation, out location);

            if (control.ParentView == view)
            {
                GUIAnimation currentAnim = view.CurrentAnimation;
                uint currentFrame = currentAnim.Frame;

                foreach (GUIAnimation animation in view.Animations)
                {
                    uint oldFrame = animation.Frame;
                    GUIAnimationChannel channel = animation.GetAnimationChannel(control);

                    if (channel == null)
                        continue;

                    view.CurrentAnimation = animation;

                    foreach (KeyFrame keyFrame in channel.KeyFrames)
                    {
                        // This will update the control
                        view.CurrentAnimation.Frame = keyFrame.Frame;

                        PointF kfCenter = control.Center;
                        float kfRotation = control.Rotation;
                        PointF kfLocation = control.Location;

                        Matrix kfTransform = control.Transform * toLocal;

                        GetComponents(control, ref kfTransform, out kfCenter, out kfRotation, out kfLocation);

                        keyFrame.Layout.Center = kfCenter;
                        keyFrame.Layout.Rotation = kfRotation;
                        keyFrame.Layout.Location = kfLocation;
                    }

                    animation.Frame = oldFrame;
                }

                view.CurrentAnimation = currentAnim;
                view.CurrentAnimation.Frame = currentFrame;

                view.SetNewParent(control, newParent, index);
            }
            else
            {
                control.ParentView.RemoveControl(control);
                view.AddControl(newParent, control);
            }

            control.Center = center;
            control.Rotation = rotation;
            control.Location = location;
        }

        /// <summary>
        /// Retrieves the components of the control's new local transform 
        /// </summary>
        /// <param name="contro  l"></param>
        /// <param name="newLocalTransform"></param>
        /// <param name="center"></param>
        /// <param name="rotation"></param>
        /// <param name="location"></param>
        private static void GetComponents(GUIControl control, ref Matrix newLocalTransform, out PointF center, out float rotation, out PointF location)
        {
            center = control.Center;
            rotation = ((float)Math.Atan2(newLocalTransform.M21, newLocalTransform.M11) / (float)Math.PI) * 180.0f;
            // atan2 -> [-pi, pi]

            // If the signs of the new rotation and old one mismatch, we need to add or subtract
            // 2 * pi to get the 'correct' rotation back
            int signA = Math.Sign(rotation);
            int signB = Math.Sign(control.Rotation);

            if (signA != 0 && signB != 0 && signA != signB)
            {
                rotation += 360.0f * Math.Sign(control.Rotation);
            }

            // The original rotation could have been expressed in multiples of 2*pi, so let's figure out what that multiplier
            // is and add that back into the rotation
            float piMult = ((int)(control.Rotation / 360.0f)) * 360.0f;
            rotation += piMult;

            Matrix mtxOffset = Matrix.Translation(-center.X, -center.Y, 0.0f);
            Matrix mtxRotation = Matrix.RotationZ((rotation / 180.0f) * (float)Math.PI);
            Matrix mtxLocation = Matrix.Invert(mtxOffset * mtxRotation) * newLocalTransform;

            Vector4 v4 = Vector4.Transform(new Vector4(0.0f, 0.0f, 0.0f, 1.0f), mtxLocation);
            location = new PointF(v4.X, v4.Y);
        }

        public override string ToString()
        {
            return "Change Parent: " + mControls.Count + " Controls";
        }
    }
}
