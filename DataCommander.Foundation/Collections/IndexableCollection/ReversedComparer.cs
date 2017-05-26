using System;
using System.Collections.Generic;

namespace Foundation.Collections.IndexableCollection
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class ReversedComparer<T> : IComparer<T>
    {
        private static readonly Lazy<ReversedComparer<T>> Instance =
            new Lazy<ReversedComparer<T>>(CreateReversedComparer);

        private readonly IComparer<T> _comparer;

        private ReversedComparer(IComparer<T> comparer)
        {
            this._comparer = comparer;
        }

        /// <summary>
        /// 
        /// </summary>
        public static IComparer<T> Default => Instance.Value;

        #region IComparer<T> Members

        int IComparer<T>.Compare(T x, T y)
        {
            return this._comparer.Compare(y, x);
        }

        #endregion

        private static ReversedComparer<T> CreateReversedComparer()
        {
            var comparer = Comparer<T>.Default;
            return new ReversedComparer<T>(comparer);
        }
    }
}