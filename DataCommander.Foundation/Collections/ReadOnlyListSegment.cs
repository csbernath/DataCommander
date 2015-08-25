namespace DataCommander.Foundation.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal sealed class ReadOnlyListSegment<T> : IReadOnlyList<T>
    {
        #region Private Fields

        private readonly IReadOnlyList<T> list;
        private readonly int offset;
        private readonly int count;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public ReadOnlyListSegment(IReadOnlyList<T> list, int offset, int count)
        {
            Contract.Requires<ArgumentNullException>(list != null);
            Contract.Requires<ArgumentOutOfRangeException>(offset >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(count >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(0 <= offset && offset < list.Count);
            Contract.Requires<ArgumentOutOfRangeException>(0 <= offset + count && offset + count <= list.Count);

            this.list = list;
            this.offset = offset;
            this.count = count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                return this.list[offset + index];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get
            {
                return this.count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            int end = offset + count;

            for (int i = offset; i < end; i++)
            {
                yield return this.list[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}