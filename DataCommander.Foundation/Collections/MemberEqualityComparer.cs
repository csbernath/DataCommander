namespace DataCommander.Foundation.Collections
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T1"></typeparam>
    public sealed class MemberEqualityComparer<T, T1> : IEqualityComparer<T>
    {
        private readonly Func<T, T1> get;
        private readonly IEqualityComparer<T1> equalityComparer;

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

            this.get = get;
            this.equalityComparer = equalityComparer;
        }

        bool IEqualityComparer<T>.Equals(T x, T y)
        {
            var x1 = this.get(x);
            var y1 = this.get(y);
            var equals = this.equalityComparer.Equals(x1, y1);
            return equals;
        }

        int IEqualityComparer<T>.GetHashCode(T obj)
        {
            var obj1 = this.get(obj);
            return this.equalityComparer.GetHashCode(obj1);
        }
    }
}