using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Otter.Containers
{
    /// <summary>
    /// Extension of the List<> class to add notifications whenever items are
    /// added/removed
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NotifyingList<T> : List<T>
    {
        public delegate void ListEventHandler(object sender, T item);

        public event ListEventHandler OnItemAdded = null;
        public event ListEventHandler OnItemRemoved = null;
        public bool SuppressEvents { get; set; }

        public NotifyingList()
            : base()
        {
        }

        public NotifyingList(IEnumerable<T> collection)
            : base(collection)
        {
        }

        public new void Add(T item)
        {
            base.Add(item);

            if (!SuppressEvents && OnItemAdded != null)
                OnItemAdded(this, item);
        }

        public new void Insert(int index, T item)
        {
            base.Insert(index, item);

            if (!SuppressEvents && OnItemAdded != null)
                OnItemAdded(this, item);
        }

        public new bool Remove(T item)
        {
            if (base.Remove(item))
            {
                if (!SuppressEvents && OnItemRemoved != null)
                    OnItemRemoved(this, item);

                return true;
            }

            return false;
        }

        public new void Clear()
        {
            List<T> clearedItems = new List<T>(this);
            base.Clear();

            foreach (T item in clearedItems)
            {
                if (!SuppressEvents && OnItemRemoved != null)
                    OnItemRemoved(this, item);
            }
        }

        public new void AddRange(IEnumerable<T> collection)
        {
            base.AddRange(collection);

            foreach (T item in collection)
            {
                if (!SuppressEvents && OnItemAdded != null)
                    OnItemAdded(this, item);
            }
        }
    }
}
