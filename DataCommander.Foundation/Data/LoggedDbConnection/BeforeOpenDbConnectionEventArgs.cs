namespace DataCommander.Foundation.Data
{
#if FOUNDATION_3_5

#else

#endif

    /// <summary>
    /// 
    /// </summary>
    public sealed class BeforeOpenDbConnectionEventArgs : LoggedEventArgs
    {
        private readonly string connectionString;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        public BeforeOpenDbConnectionEventArgs( string connectionString )
        {
            this.connectionString = connectionString;
        }

        /// <summary>
        /// 
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return this.connectionString;
            }
        }
    }
}