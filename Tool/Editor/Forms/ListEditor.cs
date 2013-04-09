using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;

namespace Otter.Editor.Forms
{
    /// <summary>
    /// List editor based on the property collection editor, because invoking
    /// the System.ComponentModel.ListEditor without a propertygrid is non-trivial
    /// </summary>
    public partial class ListEditor<T> : Form where T : class
    {
        #region Events and Delegates
        /// <summary>
        /// Arguments for collection events
        /// </summary>
        public class ListEditorEventArgs : EventArgs
        {
            #region Data
            private object mObject = null;
            #endregion

            #region Properties
            /// <summary>
            /// Gets the object associated with this event
            /// </summary>
            public object Object
            {
                get { return mObject; }
            }

            /// <summary>
            /// If set, cancels the action that caused the event
            /// </summary>
            public bool CancelAction
            {
                get;
                set;
            }

            #endregion

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="obj"></param>
            public ListEditorEventArgs(object obj)
            {
                mObject = obj;
            }
        }

        public delegate void ListEditorEventHandler(object sender, ListEditorEventArgs e);

        public event ListEditorEventHandler AddingItem = null;
        public event ListEditorEventHandler RemovingItem = null;
        #endregion

        #region Properties
        /// <summary>
        /// Gets / Sets the collection to edit
        /// </summary>
        public List<T> List
        {
            get 
            {
                T[] array = new T[mItemsListBox.Items.Count];
                mItemsListBox.Items.CopyTo(array, 0);

                return new List<T>(array); 
            }
        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public ListEditor(List<T> collection)
        {
            InitializeComponent();

            if (collection != null)
            {
                foreach (T t in collection)
                {
                    mItemsListBox.Items.Add(t);
                }
            }
        }

        /// <summary>
        /// Called when the editor is loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListEditor_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Called when an object is about to be added to the list
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool OnAddingItem(ListEditorEventArgs e)
        {
            if (AddingItem != null)
            {
                AddingItem(this, e);

                if (e.CancelAction)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Called when an object is about to be removed from the list
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool OnRemovingItem(ListEditorEventArgs e)
        {
            if (RemovingItem != null)
            {
                RemovingItem(this, e);

                if (e.CancelAction)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Adds a new instance of the object to array
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mAddButton_Click(object sender, EventArgs e)
        {
            System.Reflection.ConstructorInfo constructorInfo = typeof(T).GetConstructor(System.Type.EmptyTypes);
            T newT = (T)constructorInfo.Invoke(null);
            if (newT != null)
            {
                if (OnAddingItem(new ListEditorEventArgs(newT)))
                {
                    mItemsListBox.SelectedIndices.Clear();
                    mItemsListBox.SelectedIndex = mItemsListBox.Items.Add(newT);
                }
            }
        }

        /// <summary>
        /// Called when the list box selected index has changed.  Simply update the property grid
        /// appropriately.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mCollectionItemsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mItemsListBox.SelectedItems.Count > 0)
            {
                T[] array = new T[mItemsListBox.SelectedItems.Count];
                mItemsListBox.SelectedItems.CopyTo(array, 0);

                mPropertyGrid.SelectedObjects = array;
                mRemoveButton.Enabled = true;
            }
            else
            {
                mPropertyGrid.SelectedObjects = null;
                mRemoveButton.Enabled = false;
            }
        }

        /// <summary>
        /// Removes an item from the collection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mRemoveButton_Click(object sender, EventArgs e)
        {
            List<T> toRemove = new List<T>(mItemsListBox.SelectedItems.Count);

            foreach (T t in mItemsListBox.SelectedItems)
            {
                if (OnRemovingItem(new ListEditorEventArgs(t)))
                {
                    toRemove.Add(t);
                }
            }

            foreach (T t in toRemove)
            {
                mItemsListBox.Items.Remove(t);
            }
        }

        /// <summary>
        /// User clicked the "OK" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mOKButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// Called when a property value has changed
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        private void mPropertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            T[] array = null;

            if (mItemsListBox.SelectedItems.Count > 0)
            {
                array = new T[mItemsListBox.SelectedItems.Count];
                mItemsListBox.SelectedItems.CopyTo(array, 0);
            }

            for(int i = 0; i < mItemsListBox.Items.Count; i++)
            {
                T t = mItemsListBox.Items[i] as T;
                mItemsListBox.Items.RemoveAt(i);
                mItemsListBox.Items.Insert(i, t);
            }

            for (int i = 0; i < array.Count(); i++)
                mItemsListBox.SelectedItems.Add(array[i]);
        }
    }
}
