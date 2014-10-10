namespace DataCommander.Foundation.Data
{
    using System;
    using System.Data;

    /// <summary>
    /// 
    /// </summary>
    public interface IDataReaderContext : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        IDbCommand Command
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        IDataReader DataReader
        {
            get;
        }
    }
}