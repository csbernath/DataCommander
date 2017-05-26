using System.Collections;
using System.Collections.Generic;

namespace Foundation.Collections
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
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
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(list != null);
            Contract.Requires<ArgumentOutOfRangeException>(offset >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(count >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(0 <= offset && offset < list.Count);
            Contract.Requires<ArgumentOutOfRangeException>(0 <= offset + count && offset + count <= list.Count);
#endif

            this._list = list;
            this._offset = offset;
            this.Count = count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index] => this._list[_offset + index];

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
                yield return this._list[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}