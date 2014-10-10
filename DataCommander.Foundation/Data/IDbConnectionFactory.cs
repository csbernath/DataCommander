namespace DataCommander.Foundation.Data
{
    using System;
    using System.Data;
    using DataCommander.Foundation.Threading;

    /// <summary>
    /// 
    /// </summary>
    public interface IDbConnectionFactory
    {
        /// <summary>
        /// 
        /// </summary>
        WorkerThread Thread
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        IDbConnection CreateConnection(
            String connectionString,
            String userName,
            String hostName);

        /// <summary>
        /// 
        /// </summary>
        IDbConnectionHelper CreateConnectionHelper(IDbConnection connection);
    }
}