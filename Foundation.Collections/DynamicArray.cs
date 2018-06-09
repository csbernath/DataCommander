using System;
using System.Collections;
using System.Collections.Generic;
using Foundation.Assertions;
using Foundation.Linq;

namespace Foundation.Collections
{
    /// <summary>
    /// https://en.wikipedia.org/wiki/Dynamic_array
    /// </summary>
    public class DynamicArray<T> : IList<T>
    {
        private readonly int _maxSize;
        private T[] _array;

        public DynamicArray(int initialSize, int maxSize)
        {
            _array = new T[initialSize];
            _maxSize = maxSize;
        }

        #region IList<T> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(T item)
        {
            return _array.IndexOf(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        void IList<T>.Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        void IList<T>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get => _array[index];

            set => _array[index] = value;
        }

        #endregion

        #region ICollection<T> Members

        public void Add(T item)
        {
            Assert.IsValidOperation(Count < _maxSize);

            if (Count == _array.Length)
            {
                var newSize = Count == 0 ? 1 : 2 * Count;

                if (newSize > _maxSize)
                    newSize = _maxSize;

                if (newSize > Count)
                {
                    var newArray = new T[newSize];
                    Array.Copy(_array, newArray, _array.Length);
                    _array = newArray;
                }
            }

            _array[Count] = item;
            Count++;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            if (Count > 0)
                Array.Clear(_array, 0, Count);

            Count = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            return _array.Contains(item);
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex) => throw new NotImplementedException();

        /// <summary>
        /// 
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsReadOnly => _array.IsReadOnly;

        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IEnumerable<T> Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            for (var i = 0; i < Count; i++)
                yield return _array[i];
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }
}