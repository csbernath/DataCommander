namespace DataCommander.Foundation.Collections
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class MultipleMemberEqualityComparer<T> : IEqualityComparer<T>
    {
        #region Private Fields

        private readonly IEqualityComparer<T>[] _equalityComparers;

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="equalityComparers"></param>
        public MultipleMemberEqualityComparer(params IEqualityComparer<T>[] equalityComparers)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(equalityComparers != null);
            Contract.Requires<ArgumentOutOfRangeException>(equalityComparers.Length > 0);
            Contract.Requires<ArgumentNullException>(Contract.ForAll(equalityComparers, c => c != null));
#endif

            this._equalityComparers = equalityComparers;
        }

        #endregion

        #region IEqualityComparer<T> Members

        bool IEqualityComparer<T>.Equals(T x, T y)
        {
            return this._equalityComparers.All(c => c.Equals(x, y));
        }

        int IEqualityComparer<T>.GetHashCode(T obj)
        {
            var hashCodes = this._equalityComparers.Select(c => c.GetHashCode(obj));
            var hashCode = hashCodes.Aggregate(CombineHashCodes);
            return hashCode;
        }

        #endregion

        #region Private Methods

        private static int CombineHashCodes(int h1, int h2)
        {
            return (h1 << 5) + h1 ^ h2;
        }

        #endregion
    }
}