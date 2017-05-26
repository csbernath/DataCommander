using System;
using System.Collections.Generic;

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
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(get != null);
            Contract.Requires<ArgumentNullException>(equalityComparer != null);
#endif

            this._get = get;
            this._equalityComparer = equalityComparer;
        }

        bool IEqualityComparer<T>.Equals(T x, T y)
        {
            var x1 = this._get(x);
            var y1 = this._get(y);
            var equals = this._equalityComparer.Equals(x1, y1);
            return equals;
        }

        int IEqualityComparer<T>.GetHashCode(T obj)
        {
            var obj1 = this._get(obj);
            return this._equalityComparer.GetHashCode(obj1);
        }
    }
}