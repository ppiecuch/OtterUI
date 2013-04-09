using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Drawing;
using System.Linq;

using System.IO;

using Otter.Export;
using Otter.UI.Animation;

namespace Otter.UI
{
    /// <summary>
    /// TODO:  Convert the Control List into a proper Collection
    /// TODO:  Convert the GUI Animation Set list into a propery Collection
    /// TODO:  Move control/set adding and removing notifiers into the appropriate collections
    /// </summary>
    public class GUIView : GUIControl
    {
        #region Events and Delegates
        public delegate void AnimationSetEventHandler(object sender, GUIAnimation animation);
        public delegate void ControlEventHandler(object sender, GUIControl control);

        public event AnimationSetEventHandler AnimationSetAdded = null;
        public event AnimationSetEventHandler AnimationSetRemoved = null;
        #endregion

        #region Data
        private GUIAnimationCollection mAnimationCollection = new GUIAnimationCollection();

        private int mCurrentAnimationIndex = 0;
        private int mNextControlID = 0;
        #endregion

        #region Hidden Properties
        [Browsable(false)]
        [XmlAttribute]
        public new int ID
        {
            get { return base.ID; }
            set { base.ID = value; }
        }

        [Browsable(false)]
        public new AnchorFlags AnchorFlags
        {
            get { return base.AnchorFlags; }
            set { base.AnchorFlags = value; }
        }

        [Browsable(false)]
        public new float Left
        {
            get { return base.Left; }
            set { base.Left = value; }
        }

        [Browsable(false)]
        public new float Right
        {
            get { return base.Right; }
            set { base.Right = value; }
        }

        [Browsable(false)]
        public new float Top
        {
            get { return base.Top; }
            set { base.Top = value; }
        }

        [Browsable(false)]
        public new float Bottom
        {
            get { return base.Bottom; }
            set { base.Bottom = value; }
        }

        [Browsable(false)]
        public new PointF Location
        {
            get { return base.Location; }
            set { base.Location = value; }
        }

        [Browsable(false)]
        public new SizeF Size
        {
            get { return base.Size; }
            set { base.Size = value; }
        }

        [Browsable(false)]
        public new float Rotation
        {
            get { return base.Rotation; }
            set { base.Rotation = value; }
        }

        [Browsable(false)]
        [XmlAttribute]
        public new bool Locked
        {
            get { return base.Locked; }
            set { base.Locked = value; }
        }

        [Browsable(false)]
        [XmlAttribute]
        public new bool Hidden
        {
            get { return base.Hidden; }
            set { base.Hidden = value; }
        }

        [Browsable(false)]
        [XmlAttribute]
        public new int Mask
        {
            get { return base.Mask; }
            set { base.Mask = value; }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets / Sets the parent scene for this view
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public override GUIScene Scene
        {
            get
            {
                return base.Scene;
            }
            set
            {
                base.Scene = value;

                // Fix up the actions to point to the parent scene
                foreach (GUIAnimation animation in Animations)
                {
                    animation.Scene = this.Scene;
                    foreach (MainChannelFrame mainChannelFrame in animation.MainChannelFrames)
                    {
                        mainChannelFrame.Animation = animation;
                        foreach (Otter.UI.Actions.Action action in mainChannelFrame.Actions)
                        {
                            action.Scene = this.Scene;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the list of used textures
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public override List<int> TextureIDs
        {
            get
            {
                List<int> list = new List<int>();
                foreach(GUIControl control in Controls)
                {
                    foreach (int texID in control.TextureIDs)
                    {
                        if (!list.Contains(texID))
                            list.Add(texID);
                    }
                }

                return list;
            }
        }

        /// <summary>
        /// Gets the list of used sounds
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public override List<int> SoundIDs
        {
            get
            {
                List<int> list = new List<int>();
                foreach (GUIAnimation animation in Animations)
                {
                    foreach (MainChannelFrame mainChannelFrame in animation.MainChannelFrames)
                    {
                        foreach (Actions.SoundAction soundAction in mainChannelFrame.Actions.OfType<Actions.SoundAction>())
                        {
                            if (soundAction.Sound != -1 && !list.Contains(soundAction.Sound))
                                list.Add(soundAction.Sound);
                        }
                    }
                }

                foreach (GUIControl control in Controls)
                {
                    foreach (int soundID in control.SoundIDs)
                    {
                        if (soundID != -1 && !list.Contains(soundID))
                            list.Add(soundID);
                    }
                }

                return list;
            }
        }

        /// <summary>
        /// Gets / Sets the list of channel sets
        /// </summary>
        [Browsable(false)]
        public GUIAnimationCollection Animations
        {
            get { return mAnimationCollection; }
            set { mAnimationCollection = value; }
        }

        /// <summary>
        /// Gets the index of the current channel
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public int CurrentAnimationIndex
        {
            get { return mCurrentAnimationIndex; }
            set { mCurrentAnimationIndex = value; }
        }

        /// <summary>
        /// Gets / Sets the current channel
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public GUIAnimation CurrentAnimation
        {
            get 
            {
                if (mCurrentAnimationIndex >= Animations.Count)
                    return null;

                return Animations[mCurrentAnimationIndex]; 
            }
            set 
            {
                mCurrentAnimationIndex = Animations.IndexOf(value); 
            }
        }

        /// <summary>
        /// Gets / Sets the next ID that will be assigned to a control
        /// </summary>
        [Browsable(false)]
        [XmlAttribute]
        public int NextControlID
        {
            get { return mNextControlID; }
            set { mNextControlID = value; }
        }
        #endregion

        #region Event Notifiers

        /// <summary>
        /// Notifies that an channel set has been added
        /// </summary>
        /// <param name="control"></param>
        private void NotifyAnimationAdded(GUIAnimation animation)
        {
            if (AnimationSetAdded != null)
                AnimationSetAdded(this, animation);
        }
        
        /// <summary>
        /// Notifies that an channel set has been removed
        /// </summary>
        /// <param name="control"></param>
        private void NotifyAnimationRemoved(GUIAnimation animation)
        {
            if (AnimationSetRemoved != null)
                AnimationSetRemoved(this, animation);
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Called when a control is added to the sceneView's collection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="control"></param>
        void Controls_OnControlAdded(object sender, GUIControl control)
        {
            control.Parent = this;
        }

        /// <summary>
        /// Called when a control is removed from the sceneView's collection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="control"></param>
        void Controls_OnControlRemoved(object sender, GUIControl control)
        {
        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public GUIView(string name)
        {
            Name = name;
            Controls.OnControlAdded += new GUIControlCollection.ControlEventHandler(Controls_OnControlAdded);
            Controls.OnControlRemoved += new GUIControlCollection.ControlEventHandler(Controls_OnControlRemoved);

            GUIAnimation onActivate = new Otter.UI.Animation.GUIAnimation("OnActivate", true);
            GUIAnimation onDeactivate = new Otter.UI.Animation.GUIAnimation("OnDeactivate", true);
            onDeactivate.NumFrames = 1;

            Animations.Add(onActivate);
            Animations.Add(onDeactivate);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public GUIView()
        {
            Controls.OnControlAdded += new GUIControlCollection.ControlEventHandler(Controls_OnControlAdded);
            Controls.OnControlRemoved += new GUIControlCollection.ControlEventHandler(Controls_OnControlRemoved);
        }


        /// <summary>
        /// After import (from xml) fixes up anything that needs to be.
        /// </summary>
        public void PostImport()
        {
            // Now cycle through the channel sets and animations, and fix up the control
            // reference
            foreach (GUIAnimation animation in Animations)
            {
                List<GUIAnimationChannel> newChannels = new List<GUIAnimationChannel>();
                foreach (GUIAnimationChannel channel in animation.AnimationChannels)
                {
                    channel.Control = Controls.GetControl(channel.ControlID);

                    if (channel.Control != null)
                        newChannels.Add(channel);
                }

                animation.AnimationChannels = newChannels;
            }
        }

        /// <summary>
        /// Creates a new keyframe for the control on the selected frame.  
        /// </summary>
        /// <param name="control"></param>
        public void CreateKeyFrame(GUIControl control)
        {
            if (CurrentAnimation == null)
                return;

            GUIAnimation targetSet = CurrentAnimation;

            // Now we have a target set.  See if this control is in the set, and if not, add it.
            CurrentAnimation.CreateKeyFrame(control, targetSet.Frame);
        }

        /// <summary>
        /// Adds a control to the sceneView
        /// </summary>
        /// <param name="control"></param>
        public void AddControl(GUIControl parent, GUIControl control)
        {
            AddControl(parent, control, -1);
        }

        /// <summary>
        /// Adds a control as the child of another.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="control"></param>
        /// <param name="atIndex"></param>
        public void AddControl(GUIControl parent, GUIControl control, int atIndex)
        {
            GUIControlCollection collection = (parent != null ? parent.Controls : this.Controls);

            if (!collection.Contains(control, true))
            {
                // We need create a keyframe for this control on the "OnActivate" channel
                GUIAnimation onActivate = Animations["OnActivate"];
                if (onActivate != null)
                    onActivate.CreateKeyFrame(control, 0);

                control.Parent = (parent != null) ? parent : this;

                if (atIndex >= 0)
                    collection.Insert(atIndex, control);
                else
                    collection.Add(control);
            }
        }

        /// <summary>
        /// Removes a control
        /// </summary>
        /// <param name="control"></param>
        public void RemoveControl(GUIControl control)
        {
            GUIControlCollection collection = (control.Parent != null ? control.Parent.Controls : Controls);
            if (collection.Contains(control, true))
            {
                // Now cycle through the active sceneView's animations and remove
                // all animations for this control.
                int cnt = Animations.Count;
                for (int i = cnt - 1; i >= 0; i--)
                {
                    GUIAnimation animation = Animations[i];
                    if (animation.GetAnimationChannel(control) != null)
                        animation.RemoveAnimationChannel(control);
                }

                control.Parent = null;
                collection.Remove(control, true);
            }

            if (control.GetType() == typeof(GUIMask))
            {
                //remove any reference to this mask from other controls
                foreach (GUIControl thisControl in Controls)
                {
                    if (thisControl.Mask == control.ID)
                        thisControl.Mask = -1;
                }
            }
        }

        public void SetNewParent(GUIControl child, GUIControl parent, int index)
        {
            if (child.ParentView != this)
                return;

            child.Parent.Controls.Remove(child, false);

            if (index >= 0 && index < parent.Controls.Count)
                parent.Controls.Insert(index, child);
            else
                parent.Controls.Add(child);
        }

        /// <summary>
        /// Adds an channel set
        /// </summary>
        /// <param name="set"></param>
        public void AddAnimation(GUIAnimation animation)
        {
            if (!Animations.Contains(animation))
            {
                Animations.Add(animation);
                NotifyAnimationAdded(animation);
            }
        }

        /// <summary>
        /// Removes an channel set
        /// </summary>
        /// <param name="set"></param>
        public void RemoveAnimation(GUIAnimation animation)
        {
            if (Animations.Contains(animation))
            {
                if (mCurrentAnimationIndex == Animations.IndexOf(animation))
                    mCurrentAnimationIndex = -1;

                Animations.Remove(animation);
                NotifyAnimationRemoved(animation);
            }
        }

        /// <summary>
        /// Exports the GUI ActiveView
        /// </summary>
        /// <param name="bw"></param>
        public override void Export(PlatformBinaryWriter bw)
        {
            FourCCStack fourCCStack = new FourCCStack(bw);
            // [G]ame[G]UI [V]ie[W]
            fourCCStack.Push("GGVW");

            base.Export(bw);

            {
                List<int> textureList = TextureIDs;
                List<int> soundList = SoundIDs;

                bw.Write(textureList.Count);
                bw.Write(soundList.Count);
                bw.Write(Controls.Count);

                int textureOffsetPos = (int)bw.BaseStream.Position;
                bw.Write(0); // Placeholder - texture data start

                int soundsOffsetPos = (int)bw.BaseStream.Position;
                bw.Write(0); // Placeholder - sound data start

                int controlsOffsetPos = (int)bw.BaseStream.Position;
                bw.Write(0); // Placeholder - control data start

                int animationListOffsetPos = (int)bw.BaseStream.Position;
                bw.Write(0); // Placeholder - channel list data start

                int dataStartPos = (int)bw.BaseStream.Position;
                int pos = 0;

                // Textures
                pos = (int)bw.BaseStream.Position;
                bw.Seek(textureOffsetPos, SeekOrigin.Begin);
                bw.Write(pos - dataStartPos);
                bw.Seek(0, SeekOrigin.End);
                foreach (int texID in textureList)
                {
                    bw.Write(Scene.GetUniqueTextureID(texID));
                }

                // Sounds
                pos = (int)bw.BaseStream.Position;
                bw.Seek(soundsOffsetPos, SeekOrigin.Begin);
                bw.Write(pos - dataStartPos);
                bw.Seek(0, SeekOrigin.End);
                foreach (int soundID in soundList)
                {
                    bw.Write(Scene.GetUniqueSoundID(soundID));
                }

                // Controls
                pos = (int)bw.BaseStream.Position;
                bw.Seek(controlsOffsetPos, SeekOrigin.Begin);
                bw.Write(pos - dataStartPos);
                bw.Seek(0, SeekOrigin.End);
                foreach (GUIControl control in Controls)
                {
                    control.Export(bw);
                }

                // Animation List
                pos = (int)bw.BaseStream.Position;
                bw.Seek(animationListOffsetPos, SeekOrigin.Begin);
                bw.Write(pos - dataStartPos);
                bw.Seek(0, SeekOrigin.End);

                // [G]ame[G]UI [A]nimation [L]ist
                fourCCStack.Push("GGAL");
                {
                    bw.Write(Animations.Count);

                    foreach (GUIAnimation animation in Animations)
                    {
                        List<GUIAnimationChannel> channels = null;

                        try
                        {
                            channels = animation.GetSortedChannels(this.Controls);
                        }
                        catch (Exception e)
                        {
                            System.Console.WriteLine("Exception : " + e);
                            channels = animation.AnimationChannels;
                        }

                        // [G]ame[G]UI [A][N]imation
                        fourCCStack.Push("GGAN");
                        {
                            byte[] bytes = Utils.StringToBytes(animation.Name, 64);
                            bytes[63] = 0;
                            bw.Write(bytes, 0, 64);
                            bw.Write(animation.NumFrames);
                            bw.Write(animation.RepeatStart);
                            bw.Write(animation.RepeatEnd);

                            bw.Write(channels.Count);
                            bw.Write(animation.MainChannelFrames.Count);

                            int actionlistDataOffsetPos = (int)bw.BaseStream.Position;
                            bw.Write(0); // Placeholder - action list data start;

                            int anmDataStartPos = (int)bw.BaseStream.Position;
                            int anmPos = 0;

                            foreach (GUIAnimationChannel channel in channels)
                            {
                                // [G]ame[G]UI [A]nimation [C]hannel
                                fourCCStack.Push("GGAC");
                                {
                                    bw.Write(channel.ControlID);
                                    bw.Write(channel.KeyFrames.Count);

                                    // Make sure we sort the keyframes prior to export
                                    channel.KeyFrames.Sort((a, b) => ((int)a.Frame - (int)b.Frame));

                                    foreach (KeyFrame keyFrame in channel.KeyFrames)
                                        keyFrame.Export(bw);
                                }
                                fourCCStack.Pop();
                            }

                            anmPos = (int)bw.BaseStream.Position;
                            bw.Seek(actionlistDataOffsetPos, SeekOrigin.Begin);
                            bw.Write(anmPos - anmDataStartPos);
                            bw.Seek(0, SeekOrigin.End);

                            foreach (MainChannelFrame mainChannelFrame in animation.MainChannelFrames)
                                mainChannelFrame.Export(bw);
                        }
                        fourCCStack.Pop();
                    }
                }
                fourCCStack.Pop();
            }
            fourCCStack.Pop();
        }

        /// <summary>
        /// Abstract clone function intended for derived objects to implement.
        /// </summary>
        /// <returns>
        /// Deep copy of this object.
        /// </returns>
        public override object Clone()
        {
            GUIView view = new GUIView();

            // Control-level layout
            view.ID = -1;
            view.Name = Name;
            view.Scene = Scene;
            view.Parent = Parent;
            view.Layout = (ControlLayout)this.Layout.Clone();

            foreach (GUIControl control in Controls)
            {
                GUIControl clone = (GUIControl)control.Clone();
                clone.Scene = this.Scene;
                clone.Parent = view;

                view.Controls.Add(clone);
            }

            view.NextControlID = this.NextControlID;

            foreach (GUIAnimation anim in Animations)
            {
                GUIAnimation cloneAnim = (GUIAnimation)anim.Clone();
                foreach (GUIAnimationChannel channel in cloneAnim.AnimationChannels)
                    channel.Control = view.Controls.GetControl(channel.ControlID);

                view.Animations.Add(cloneAnim);
            }

            return view;
        }
    }
}
