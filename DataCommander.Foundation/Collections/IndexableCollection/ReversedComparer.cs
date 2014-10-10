namespace DataCommander.Foundation.Collections
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class ReversedComparer<T> : IComparer<T>
    {
        private static readonly Lazy<ReversedComparer<T>> Instance = new Lazy<ReversedComparer<T>>( CreateReversedComparer );
        private readonly IComparer<T> comparer;

        private ReversedComparer( IComparer<T> comparer )
        {
            this.comparer = comparer;
        }

        /// <summary>
        /// 
        /// </summary>
        public static IComparer<T> Default
        {
            get
            {
                return Instance.Value;
            }
        }

        #region IComparer<T> Members

        Int32 IComparer<T>.Compare( T x, T y )
        {
            return this.comparer.Compare( y, x );
        }

        #endregion

        private static ReversedComparer<T> CreateReversedComparer()
        {
            var comparer = Comparer<T>.Default;
            return new ReversedComparer<T>( comparer );
        }
    }
}