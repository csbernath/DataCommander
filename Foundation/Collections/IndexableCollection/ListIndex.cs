using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Foundation.Diagnostics.Assertions;
using Foundation.Diagnostics.Contracts;

namespace Foundation.Collections.IndexableCollection
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListIndex<T> : ICollectionIndex<T>, IList<T>
    {
        private IList<T> _list;

        /// <summary>
        /// 
        /// </summary>
        public ListIndex(string name)
        {
            Assert.IsNotNull(name);

            Initialize(name, new List<T>());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="list"></param>
        public ListIndex(string name, IList<T> list)
        {
            Assert.IsNotNull(name);
            Assert.IsNotNull(list);

            Initialize(name, list);
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int Count => _list.Count;

        bool ICollection<T>.IsReadOnly => false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                FoundationContract.Assert(index < Count);

                return _list[index];
            }

            set
            {
                FoundationContract.Assert(index < Count);

                _list[index] = value;
            }
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
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, T item)
        {
            _list.Insert(index, item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        #region ICollectionIndex<T> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            _list.Add(item);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            _list.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            FoundationContract.Ensures(!Contract.Result<bool>() || Count > 0);

            return _list.Contains(item);
        }

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

        private void Initialize(string name, IList<T> list)
        {
            Assert.IsNotNull(name);
            Assert.IsNotNull(list);

            Name = name;
            _list = list;
        }
    }
}