namespace Foundation.Data.TextData
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class TextDataSetTable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="rowCount"></param>
        /// <param name="table"></param>
        public TextDataSetTable( string name, int rowCount, TextDataTable table )
        {
#if CONTRACTS_FULL
            Contract.Requires(rowCount >= 0);
#endif

            Name = name;
            RowCount = rowCount;
            Table = table;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 
        /// </summary>
        public int RowCount { get; }

        /// <summary>
        /// 
        /// </summary>
        public TextDataTable Table { get; }
    }
}