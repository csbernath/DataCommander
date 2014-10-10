namespace DataCommander.Foundation.Data
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// 
    /// </summary>
    public sealed class TextDataSetTable
    {
        private readonly String name;
        private readonly Int32 rowCount;
        private readonly TextDataTable table;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="rowCount"></param>
        /// <param name="table"></param>
        public TextDataSetTable( String name, Int32 rowCount, TextDataTable table )
        {
            Contract.Requires(rowCount >= 0);

            this.name = name;
            this.rowCount = rowCount;
            this.table = table;
        }

        /// <summary>
        /// 
        /// </summary>
        public String Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 RowCount
        {
            get
            {
                return this.rowCount;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public TextDataTable Table
        {
            get
            {
                return this.table;
            }
        }
    }
}