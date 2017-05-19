namespace DataCommander.Foundation.Collections.IndexableCollection
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public partial class IndexableCollection<T> : ICollection<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public int Count => this.defaultIndex.Count;

        /// <summary>
        /// 
        /// </summary>
        public bool IsReadOnly => this.defaultIndex.IsReadOnly;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            foreach (var index in this.Indexes)
            {
                index.Add(item);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            foreach (var index in this.Indexes)
            {
                index.Clear();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
#if CONTRACTS_FULL
            Contract.Ensures(!Contract.Result<bool>() || this.Count > 0);
#endif
            return this.defaultIndex.Contains(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            this.defaultIndex.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(T item)
        {
            return this.Indexes.All(index => index.Remove(item));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this.defaultIndex.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.defaultIndex.GetEnumerator();
        }
    }
}