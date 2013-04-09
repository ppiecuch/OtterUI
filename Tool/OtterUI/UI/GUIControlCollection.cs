using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Drawing;

namespace Otter.UI
{
    /// <summary>
    /// Simple contains a collection of controls.
    /// </summary>
    public class GUIControlCollection : ICollection
    {
        #region Events and Delegates
        public delegate void ControlEventHandler(object sender, GUIControl control);
        public event ControlEventHandler OnControlAdded = null;
        public event ControlEventHandler OnControlRemoved = null;

        /// <summary>
        /// Notifies that a control has been added to the collection
        /// </summary>
        /// <param name="control"></param>
        private void NotifyControlAdded(GUIControl control)
        {
            if (OnControlAdded != null)
                OnControlAdded(this, control);
        }

        /// <summary>
        /// Notifies that a control has been removed from the collection
        /// </summary>
        /// <param name="control"></param>
        private void NotifyControlRemoved(GUIControl control)
        {
            if (OnControlRemoved != null)
                OnControlRemoved(this, control);
        }
        #endregion

        #region Data
        private List<GUIControl> mControls = new List<GUIControl>();
        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        public GUIControlCollection()
        {
        }

        /// <summary>
        /// Constructs the control collection with an initial set of controls
        /// </summary>
        /// <param name="controls"></param>
        public GUIControlCollection(GUIControl[] controls)
        {
            mControls = new List<GUIControl>(controls);
        }

        /// <summary>
        /// Retrieves a control by count
        /// </summary>
        /// <param name="point"></param>
        /// <param name="filterList"></param>
        /// <returns></returns>
        public GUIControl GetControl(int id)
        {
            foreach (GUIControl control in mControls)
            {
                if (control == null)
                    continue;

                if (control.ID == id)
                    return control;

                GUIControl result = control.Controls.GetControl(id);

                if (result != null)
                    return result;
            }

            return null;
        }

        /// <summary>
        /// Retrieves a control by name
        /// </summary>
        /// <param name="point"></param>
        /// <param name="filterList"></param>
        /// <returns></returns>
        public GUIControl GetControl(string name)
        {
            foreach (GUIControl control in mControls)
            {
                if (control == null)
                    continue;

                if (control.Name == name)
                    return control;

                GUIControl result = control.Controls.GetControl(name);

                if (result != null)
                    return result;
            }

            return null;
        }

        /// <summary>
        /// Returns the maximum control ID in the entire collection, incl. children
        /// </summary>
        /// <returns></returns>
        public int GetMaxControlID()
        {
            int maxID = 0;
            GetMaxControlID(this, ref maxID);
            return maxID;
        }

        /// <summary>
        /// Retrieves the maximum control count in the collection of controls.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="currentMax"></param>
        private void GetMaxControlID(GUIControlCollection collection, ref int currentMax)
        {
            foreach (GUIControl control in collection)
            {
                if (control == null)
                    continue;

                if (control.ID > currentMax)
                    currentMax = control.ID;

                GetMaxControlID(control.Controls, ref currentMax);
            }
        }

        /// <summary>
        /// Returns the index of a provided control in this collection.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public int IndexOf(GUIControl control)
        {
            return mControls.IndexOf(control);
        }

        /// <summary>
        /// Returns a typed collection of specified type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> OfType<T>() where T : class
        {
            return mControls.OfType<T>();
        }

        /// <summary>
        /// Returns a generic enumerator
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return mControls.GetEnumerator();
        }        

        #region Collection Accessors
        /// <summary>
        /// Custom array indexing operator
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public GUIControl this[int i]
        {
            get 
            { 
                return (mControls[i] as GUIControl); 
            }
            set
            {
                mControls[i] = value;
            }
        }

        /// <summary>
        /// Returns the number of controls in this group
        /// </summary>
        public int Count
        {
            get { return mControls.Count; }
        }

        /// <summary>
        /// Adds a control to this group
        /// </summary>
        /// <param name="control"></param>
        public void Add(GUIControl control)
        {
            mControls.Add(control);
            NotifyControlAdded(control);
        }

        /// <summary>
        /// Inserts a control at a specific location
        /// </summary>
        /// <param name="index"></param>
        /// <param name="control"></param>
        public void Insert(int index, GUIControl control)
        {
            mControls.Insert(index, control);
            NotifyControlAdded(control);
        }

        /// <summary>
        /// Clears this group of all controls
        /// </summary>
        public void Clear()
        {
            mControls.Clear();
        }

        /// <summary>
        /// Returns whether or not we contain a specified control
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public bool Contains(GUIControl control, bool checkChildren)
        {
            if (mControls.Contains(control))
                return true;

            if(checkChildren)
            {
                foreach (GUIControl cntrl in mControls)
                {
                    if(cntrl.Controls.Contains(control, checkChildren))
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Copies to an array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(Array array, int arrayIndex)
        {
            ArrayList arrayList = new ArrayList(mControls);
            arrayList.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes an item from a control
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public bool Remove(GUIControl control, bool checkChildren)
        {
            if (mControls.Contains(control))
            {
                mControls.Remove(control);
                NotifyControlRemoved(control);
                return true;
            }

            if (checkChildren)
            {
                foreach (GUIControl cntrl in mControls)
                {
                    if (cntrl.Controls.Remove(control, checkChildren))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        #endregion

        #region ICollection Members
        public bool IsSynchronized
        {
            get 
            {
                return false;
            }
        }

        public object SyncRoot
        {
            get 
            {
                return null;
            }
        }
        #endregion
    }
}
