using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Linq;

namespace Otter.UI.Animation
{
    /// <summary>
    /// Maintains a collection of GUIAnimations
    /// </summary>
    public class GUIAnimationCollection
    {
        #region Data
        private List<GUIAnimation> mAnimations = new List<GUIAnimation>();
        #endregion

        #region Properties
        [Browsable(false)]
        public List<GUIAnimation> AnimationList
        {
            get { return mAnimations; }
            set
            {
                mAnimations = value;
            }
        }
        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        public GUIAnimationCollection()
        {
        }

        /// <summary>
        /// Constructs the channel collection with an initial set of animations
        /// </summary>
        /// <param name="states"></param>
        public GUIAnimationCollection(GUIAnimation[] animations)
        {
            mAnimations = new List<GUIAnimation>(animations);
        }

        /// <summary>
        /// Moves an channel to the back of the list
        /// </summary>
        /// <param name="channel"></param>
        public void MoveToBack(GUIAnimation animation)
        {
            if (animation == null)
                return;

            if (!Contains(animation))
                return;

            mAnimations.Remove(animation);
            mAnimations.Add(animation);
        }

        /// <summary>
        /// Returns a typed collection of specified type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> OfType<T>() where T : class
        {
            return AnimationList.OfType<T>();
        }

        #region Collection Accessors
        /// <summary>
        /// Custom array indexing operator
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public GUIAnimation this[int i]
        {
            get { return mAnimations[i]; }
            set
            {
                mAnimations[i] = value;
            }
        }

        /// <summary>
        /// Retrieves an channel set by name.
        /// TODO : Move this into a set collection, when created.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GUIAnimation this[string name]
        {
            get
            {
                foreach (GUIAnimation animation in mAnimations)
                {
                    if (animation.Name == name)
                        return animation;
                }

                return null;
            }
            set
            {
                int cnt = mAnimations.Count;
                for(int i = 0; i < cnt; i++)
                {
                    GUIAnimation anim = mAnimations[i];
                    if (anim.Name == name)
                    {
                        mAnimations[i] = value;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Returns the number of animations in this group
        /// </summary>
        public int Count
        {
            get { return mAnimations.Count; }
        }

        /// <summary>
        /// Adds an channel to this group
        /// </summary>
        /// <param name="channel"></param>
        public void Add(GUIAnimation animation)
        {
            mAnimations.Add(animation);
        }

        /// <summary>
        /// Clears this collection of all animations
        /// </summary>
        public void Clear()
        {
            mAnimations.Clear();
        }

        /// <summary>
        /// Returns whether or not we contain a specified channel
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public bool Contains(GUIAnimation animation)
        {
            return mAnimations.Contains(animation);
        }

        /// <summary>
        /// Returns the index of the channel in the collection.  Returns -1 if 
        /// not present
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public int IndexOf(GUIAnimation animation)
        {
            return mAnimations.IndexOf(animation);
        }

        /// <summary>
        /// Copies to an array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(GUIAnimation[] array, int arrayIndex)
        {
            mAnimations.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes an item from a channel
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public bool Remove(GUIAnimation animation)
        {
            bool bSuccess = mAnimations.Remove(animation);
            return bSuccess;
        }

        /// <summary>
        /// Returns an enumerator
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator<GUIAnimation> GetEnumerator()
        {
            return mAnimations.GetEnumerator();
        }

        /// <summary>
        /// Returns this group as an array
        /// </summary>
        /// <returns></returns>
        public GUIAnimation[] ToArray()
        {
            return mAnimations.ToArray();
        }
        #endregion
    }
}
