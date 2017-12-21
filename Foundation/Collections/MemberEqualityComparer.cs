using System;
using System.Collections.Generic;
using Foundation.Diagnostics.Contracts;

namespace Foundation.Collections
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T1"></typeparam>
    public sealed class MemberEqualityComparer<T, T1> : IEqualityComparer<T>
    {
        private readonly Func<T, T1> _get;
        private readonly IEqualityComparer<T1> _equalityComparer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="get"></param>
        public MemberEqualityComparer(Func<T, T1> get)
            : this(get, EqualityComparer<T1>.Default)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="get"></param>
        /// <param name="equalityComparer"></param>
        public MemberEqualityComparer(Func<T, T1> get, IEqualityComparer<T1> equalityComparer)
        {
            FoundationContract.Requires<ArgumentNullException>(get != null);
            FoundationContract.Requires<ArgumentNullException>(equalityComparer != null);

            _get = get;
            _equalityComparer = equalityComparer;
        }

        bool IEqualityComparer<T>.Equals(T x, T y)
        {
            var x1 = _get(x);
            var y1 = _get(y);
            var equals = _equalityComparer.Equals(x1, y1);
            return equals;
        }

        int IEqualityComparer<T>.GetHashCode(T obj)
        {
            var obj1 = _get(obj);
            return _equalityComparer.GetHashCode(obj1);
        }
    }
}