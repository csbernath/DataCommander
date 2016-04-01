namespace DataCommander.Foundation.Collections
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LinkedListIndex<T> : ICollectionIndex<T>
    {
        private readonly LinkedList<T> linkedList = new LinkedList<T>();

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
        public int Count => this.linkedList.Count;

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
            LinkedListNode<T> node = this.linkedList.AddLast(item);
        }

        /// <summary>
        /// 
        /// </summary>
        void ICollection<T>.Clear()
        {
            this.linkedList.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            return this.linkedList.Contains(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            this.linkedList.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool ICollection<T>.Remove( T item )
        {
            return this.linkedList.Remove(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> /*IEnumerable<T>.*/GetEnumerator()
        {
            return this.linkedList.GetEnumerator();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.linkedList.GetEnumerator();
        }
    }
}