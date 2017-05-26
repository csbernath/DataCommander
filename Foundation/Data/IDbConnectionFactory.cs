using System.Data;
using Foundation.Threading;

namespace Foundation.Data
{
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