using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Foundation.Diagnostics.Contracts;

//using Foundation.Diagnostics.Contracts;

namespace Foundation.Collections.IndexableCollection
{
    public partial class IndexableCollection<T> : ICollection<T>
    {
        /// <summary>
        /// </summary>
        public int Count => _defaultIndex.Count;

        /// <summary>
        /// </summary>
        public bool IsReadOnly => _defaultIndex.IsReadOnly;

        /// <summary>
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            foreach (var index in Indexes) index.Add(item);
        }

        /// <summary>
        /// </summary>
        public void Clear()
        {
            foreach (var index in Indexes) index.Clear();
        }

        /// <summary>
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            FoundationContract.Ensures(!Contract.Result<bool>() || Count > 0);

            return _defaultIndex.Contains(item);
        }

        /// <summary>
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            _defaultIndex.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(T item)
        {
            return Indexes.All(index => index.Remove(item));
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _defaultIndex.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _defaultIndex.GetEnumerator();
        }
    }
}