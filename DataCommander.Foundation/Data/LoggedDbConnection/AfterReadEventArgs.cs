namespace DataCommander.Foundation.Data
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class AfterReadEventArgs : LoggedEventArgs
    {
        private readonly int rowCount;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowCount"></param>
        public AfterReadEventArgs(int rowCount)
        {
            this.rowCount = rowCount;
        }

        /// <summary>
        /// 
        /// </summary>
        public int RowCount => this.rowCount;
    }
}