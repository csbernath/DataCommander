namespace DataCommander.Foundation.Text
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Text;

    /// <summary>
    /// Represents a row in the <see cref="StringTable"/>.
    /// </summary>
    public sealed class StringTableRow
    {
        private readonly string[] cells;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        internal StringTableRow(StringTable table)
        {
            Contract.Requires<ArgumentNullException>(table != null);

            this.Table = table;
            this.cells = new string[table.Columns.Count];
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
                Contract.Requires(0 <= columnIndex && columnIndex < this.Table.Columns.Count);

                return this.cells[columnIndex];
            }

            set
            {
                Contract.Requires(0 <= columnIndex && columnIndex < this.Table.Columns.Count);
                this.cells[columnIndex] = value;
            }
        }
    }
}