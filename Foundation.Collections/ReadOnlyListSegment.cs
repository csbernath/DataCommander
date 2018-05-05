using System;
using System.Collections;
using System.Collections.Generic;
using Foundation.Assertions;
using Foundation.Diagnostics.Contracts;

namespace Foundation.Collections
{
    internal sealed class ReadOnlyListSegment<T> : IReadOnlyList<T>
    {
        #region Private Fields

        private readonly IReadOnlyList<T> _list;
        private readonly int _offset;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public ReadOnlyListSegment(IReadOnlyList<T> list, int offset, int count)
        {
            Assert.IsNotNull(list);
            FoundationContract.Requires<ArgumentOutOfRangeException>(offset >= 0);
            FoundationContract.Requires<ArgumentOutOfRangeException>(count >= 0);
            FoundationContract.Requires<ArgumentOutOfRangeException>(0 <= offset && offset < list.Count);
            FoundationContract.Requires<ArgumentOutOfRangeException>(0 <= offset + count && offset + count <= list.Count);

            _list = list;
            _offset = offset;
            Count = count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index] => _list[_offset + index];

        /// <summary>
        /// 
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            var end = _offset + Count;

            for (var i = _offset; i < end; i++)
            {
                yield return _list[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}