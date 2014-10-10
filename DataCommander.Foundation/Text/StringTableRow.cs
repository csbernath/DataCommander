namespace DataCommander.Foundation.Text
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents a row in the <see cref="StringTable"/>.
    /// </summary>
    public sealed class StringTableRow
    {
        private StringTable table;
        private String[] cells;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        internal StringTableRow( StringTable table )
        {
            Contract.Requires( table != null );

            this.table = table;
            this.cells = new String[table.Columns.Count];
        }

        /// <summary>
        /// 
        /// </summary>
        public StringTable Table
        {
            get
            {
                return this.table;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String this[ Int32 columnIndex ]
        {
            get
            {
                Contract.Requires( 0 <= columnIndex && columnIndex < this.Table.Columns.Count );

                return this.cells[ columnIndex ];
            }

            set
            {
                Contract.Requires( 0 <= columnIndex && columnIndex < this.Table.Columns.Count );
                this.cells[ columnIndex ] = value;
            }
        }
    }
}