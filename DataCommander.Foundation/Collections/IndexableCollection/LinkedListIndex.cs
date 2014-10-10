namespace DataCommander.Foundation.Collections
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LinkedListIndex<T> : ICollectionIndex<T>
    {
        private readonly String name;
        private readonly LinkedList<T> linkedList = new LinkedList<T>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public LinkedListIndex(String name)
        {
            this.name = name;
        }

        /// <summary>
        /// 
        /// </summary>
        public String Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 Count
        {
            get
            {
                return this.linkedList.Count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Boolean IsReadOnly
        {
            get
            {
                return false;
            }
        }

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
        public Boolean Contains(T item)
        {
            return this.linkedList.Contains(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(T[] array, Int32 arrayIndex)
        {
            this.linkedList.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Boolean ICollection<T>.Remove( T item )
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
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.linkedList.GetEnumerator();
        }
    }
}