namespace DataCommander.Foundation.Data
{
    using System;

#if FOUNDATION_3_5

#else

#endif

    /// <summary>
    /// 
    /// </summary>
    public sealed class BeforeOpenDbConnectionEventArgs : LoggedEventArgs
    {
        private readonly String connectionString;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        public BeforeOpenDbConnectionEventArgs( String connectionString )
        {
            this.connectionString = connectionString;
        }

        /// <summary>
        /// 
        /// </summary>
        public String ConnectionString
        {
            get
            {
                return this.connectionString;
            }
        }
    }
}