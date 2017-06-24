using System;
using System.Collections.Generic;

namespace Foundation.Collections
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T1"></typeparam>
    public sealed class MemberComparer<T, T1> : IComparer<T>
    {
        private readonly Func<T, T1> _get;
        private readonly IComparer<T1> _comparer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="get"></param>
        /// <param name="comparer"></param>
        public MemberComparer(Func<T, T1> get, IComparer<T1> comparer)
        {
            _get = get;
            _comparer = comparer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="get"></param>
        public MemberComparer(Func<T, T1> get)
            : this(get, Comparer<T1>.Default)
        {
        }

        int IComparer<T>.Compare(T x, T y)
        {
            var x1 = _get(x);
            var y1 = _get(y);
            var result = _comparer.Compare(x1, y1);
            return result;
        }
    }
}