namespace Foundation.Collections
{

#if FOUNDATION_2_0 || FOUNDATION_3_5
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public sealed class TupleEqualityComparer<T1, T2> : IEqualityComparer<Tuple<T1, T2>>
    {
        private IEqualityComparer<T1> comparer1;
        private IEqualityComparer<T2> comparer2;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="comparer1"></param>
        /// <param name="comparer2"></param>
        public TupleEqualityComparer( IEqualityComparer<T1> comparer1, IEqualityComparer<T2> comparer2 )
        {
            Assert.IsNotNull( comparer1, "comparer1" );
            Assert.IsNotNull( comparer2, "comparer2" );
            this.comparer1 = comparer1;
            this.comparer2 = comparer2;
        }

    #region IEqualityComparer<Tuple<T1,T2>> Members

        Boolean IEqualityComparer<Tuple<T1, T2>>.Equals( Tuple<T1, T2> x, Tuple<T1, T2> y )
        {
            return this.comparer1.Equals( x.Item1, y.Item1 ) && this.comparer2.Equals( x.Item2, y.Item2 );
        }

        Int32 IEqualityComparer<Tuple<T1, T2>>.GetHashCode( Tuple<T1, T2> obj )
        {
            Int32 h1 = this.comparer1.GetHashCode( obj.Item1 );
            Int32 h2 = this.comparer2.GetHashCode( obj.Item2 );
            return Tuple.CombineHashCodes( h1, h2 );
        }

    #endregion
    }
#endif
}