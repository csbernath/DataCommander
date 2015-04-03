namespace DataCommander.Foundation.Data
{
    using System.Diagnostics.Contracts;

    /// <summary>
    /// 
    /// </summary>
    public sealed class TextDataSetTable
    {
        private readonly string name;
        private readonly int rowCount;
        private readonly TextDataTable table;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="rowCount"></param>
        /// <param name="table"></param>
        public TextDataSetTable( string name, int rowCount, TextDataTable table )
        {
            Contract.Requires(rowCount >= 0);

            this.name = name;
            this.rowCount = rowCount;
            this.table = table;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int RowCount
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