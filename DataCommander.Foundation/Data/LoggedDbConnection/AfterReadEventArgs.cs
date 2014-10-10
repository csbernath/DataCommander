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
        private readonly Int32 rowCount;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowCount"></param>
        public AfterReadEventArgs( Int32 rowCount )
        {
            this.rowCount = rowCount;
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
    }
}