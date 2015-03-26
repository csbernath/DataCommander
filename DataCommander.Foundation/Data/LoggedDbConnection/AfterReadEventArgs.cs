namespace DataCommander.Foundation.Data
{
    using System;

#if FOUNDATION_3_5

#else

#endif

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
        public AfterReadEventArgs( int rowCount )
        {
            this.rowCount = rowCount;
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
    }
}