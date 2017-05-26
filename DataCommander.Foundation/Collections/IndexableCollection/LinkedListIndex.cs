using System.Collections;
using System.Collections.Generic;

namespace Foundation.Collections.IndexableCollection
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LinkedListIndex<T> : ICollectionIndex<T>
    {
        private readonly LinkedList<T> _linkedList = new LinkedList<T>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public LinkedListIndex(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 
        /// </summary>
        public int Count => this._linkedList.Count;

        /// <summary>
        /// 
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        void ICollection<T>.Add( T item )
        {
            var node = this._linkedList.AddLast(item);
        }

        /// <summary>
        /// 
        /// </summary>
        void ICollection<T>.Clear()
        {
            this._linkedList.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            return this._linkedList.Contains(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            this._linkedList.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool ICollection<T>.Remove( T item )
        {
            return this._linkedList.Remove(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> /*IEnumerable<T>.*/GetEnumerator()
        {
            return this._linkedList.GetEnumerator();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._linkedList.GetEnumerator();
        }
    }
}