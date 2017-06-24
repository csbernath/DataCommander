namespace Foundation.Collections.IndexableCollection
{
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
        private readonly ICollectionIndex<T> _defaultIndex;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="defaultIndex"></param>
        public IndexableCollection(ICollectionIndex<T> defaultIndex)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(defaultIndex != null);
            Contract.Ensures(this.Indexes.Count == 1);
#endif

            _defaultIndex = defaultIndex;
            Indexes.Add(defaultIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        public IndexCollection<T> Indexes { get; } = new IndexCollection<T>();

        //[ContractInvariantMethod]
        private void ContractInvariant()
        {
#if CONTRACTS_FULL
            Contract.Invariant(this.Indexes != null);
            Contract.Invariant(this.defaultIndex != null);
            Contract.Invariant(this.Indexes.Count > 0);
#endif
        }
    }
}