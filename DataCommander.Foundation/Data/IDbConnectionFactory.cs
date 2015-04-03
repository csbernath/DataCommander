namespace DataCommander.Foundation.Data
{
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
            string connectionString,
            string userName,
            string hostName);

        /// <summary>
        /// 
        /// </summary>
        IDbConnectionHelper CreateConnectionHelper(IDbConnection connection);
    }
}