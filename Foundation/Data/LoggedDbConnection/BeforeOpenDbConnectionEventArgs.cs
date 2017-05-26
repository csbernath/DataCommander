namespace Foundation.Data.LoggedDbConnection
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class BeforeOpenDbConnectionEventArgs : LoggedEventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        public BeforeOpenDbConnectionEventArgs(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        /// <summary>
        /// 
        /// </summary>
        public string ConnectionString { get; }
    }
}