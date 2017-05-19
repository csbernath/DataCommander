namespace DataCommander.Foundation.Data.LoggedDbConnection
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class AfterReadEventArgs : LoggedEventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowCount"></param>
        public AfterReadEventArgs(int rowCount)
        {
            this.RowCount = rowCount;
        }

        /// <summary>
        /// 
        /// </summary>
        public int RowCount { get; }
    }
}