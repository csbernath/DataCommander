namespace Foundation.Text
{
    /// <summary>
    /// Represents a row in the <see cref="StringTable"/>.
    /// </summary>
    public sealed class StringTableRow
    {
        private readonly string[] _cells;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        internal StringTableRow(StringTable table)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(table != null);
#endif

            this.Table = table;
            this._cells = new string[table.Columns.Count];
        }

        /// <summary>
        /// 
        /// </summary>
        public StringTable Table { get; }

        /// <summary>
        /// 
        /// </summary>
        public string this[int columnIndex]
        {
            get
            {
#if CONTRACTS_FULL
                Contract.Requires(0 <= columnIndex && columnIndex < this.Table.Columns.Count);
#endif

                return this._cells[columnIndex];
            }

            set
            {
#if CONTRACTS_FULL
                Contract.Requires(0 <= columnIndex && columnIndex < this.Table.Columns.Count);
#endif
                this._cells[columnIndex] = value;
            }
        }
    }
}