namespace DataCommander.Foundation.Data
{
    using System.Diagnostics.Contracts;

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
            Contract.Requires(rowCount >= 0);

            this.Name = name;
            this.RowCount = rowCount;
            this.Table = table;
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