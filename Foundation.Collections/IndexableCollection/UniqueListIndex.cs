using System;
using System.Collections;
using System.Collections.Generic;
using Foundation.Assertions;
using Foundation.Diagnostics.Contracts;

namespace Foundation.Collections.IndexableCollection
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="T"></typeparam>
    public class UniqueListIndex<TKey, T> : ICollectionIndex<T>
    {
        private Func<T, TKey> _keySelector;
        private readonly IList<T> _list;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="keySelector"></param>
        /// <param name="list"></param>
        public UniqueListIndex(
            string name,
            Func<T, TKey> keySelector,
            IList<T> list)
        {
            Assert.IsNotNull(name);
            Assert.IsNotNull(keySelector);
            Assert.IsNotNull(list);

            Name = name;
            _keySelector = keySelector;
            _list = list;
        }

#region ICollectionIndex<T> Members

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; }

#endregion

#region ICollection<T> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            var contains = _list.Contains(item);

            if (contains)
            {
                throw new ArgumentException();
            }

            _list.Add(item);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            FoundationContract.Ensures(Count == 0);
            _list.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            //FoundationContract.Ensures(!Contract.Result<bool>() || this.Count > 0);
            return _list.Contains(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count => _list.Count;

        /// <summary>
        /// 
        /// </summary>
        public bool IsReadOnly => _list.IsReadOnly;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(T item)
        {
            return _list.Remove(item);
        }

#endregion

#region IEnumerable<T> Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

#endregion

#region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

#endregion
    }
}