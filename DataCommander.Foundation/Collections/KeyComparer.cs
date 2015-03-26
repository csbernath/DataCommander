namespace DataCommander.Foundation.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public sealed class KeyComparer<T, TKey> : IComparer<T>
    {
        private readonly Func<T, TKey> keySelector;
        private readonly IComparer<TKey> keyComparer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keySelector"></param>
        /// <param name="keyComparer"></param>
        public KeyComparer( Func<T, TKey> keySelector, IComparer<TKey> keyComparer )
        {
            Contract.Requires( keySelector != null );
            Contract.Requires( keyComparer != null );

            this.keySelector = keySelector;
            this.keyComparer = keyComparer;
        }

        #region IComparer<T> Members

        int IComparer<T>.Compare( T x, T y )
        {
            TKey keyX = this.keySelector( x );
            TKey keyY = this.keySelector( y );
            return this.keyComparer.Compare( keyX, keyY );
        }

        #endregion
    }
}