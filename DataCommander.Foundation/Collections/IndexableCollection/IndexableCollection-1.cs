namespace DataCommander.Foundation.Collections
{
    using System.Diagnostics.Contracts;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    /// Implemented index types:
    /// <list type="table">
    /// <listheader>
    ///         <term>heaterm1</term>
    ///         <description>headesc1</description>
    /// </listheader>
    ///     <item>
    ///         <term><see cref="UniqueIndex{TKey,T}" /></term>
    ///         <description>desc1</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="UniqueListIndex{TKey,T}" /></term>
    ///         <description>desc1</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="NonUniqueIndex{TKey,T}" /></term>
    ///         <description>desc2</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="SequenceIndex{TKey,T}" /></term>
    ///         <description></description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="ListIndex{T}" /></term>
    ///         <description>desc1</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="LinkedListIndex{T}" /></term>
    ///         <description>desc1</description>
    ///     </item>
    /// </list>
    /// </remarks>
    public partial class IndexableCollection<T>
    {
        private readonly ICollectionIndex<T> defaultIndex;
        private readonly IndexCollection<T> indexes = new IndexCollection<T>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="defaultIndex"></param>
        public IndexableCollection( ICollectionIndex<T> defaultIndex )
        {
            Contract.Requires( defaultIndex != null );
            Contract.Ensures( this.indexes.Count == 1 );

            this.defaultIndex = defaultIndex;
            this.indexes.Add( defaultIndex );
        }

        /// <summary>
        /// 
        /// </summary>
        public IndexCollection<T> Indexes
        {
            get
            {
                return this.indexes;
            }
        }

        [ContractInvariantMethod]
        private void ContractInvariant()
        {
            Contract.Invariant( this.indexes != null );
            Contract.Invariant( this.defaultIndex != null );
            Contract.Invariant( this.indexes.Count > 0 );
        }
    }
}