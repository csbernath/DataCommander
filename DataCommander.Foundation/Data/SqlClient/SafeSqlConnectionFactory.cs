namespace DataCommander.Foundation.Data.SqlClient
{
    using System.Data;
    using System.Data.SqlClient;
    using DataCommander.Foundation.Threading;

    /// <summary>
    /// 
    /// </summary>
    public class SafeSqlConnectionFactory : IDbConnectionFactory
    {
        /// <summary>
        /// 
        /// </summary>
        public WorkerThread Thread => null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="userName"></param>
        /// <param name="hostName"></param>
        /// <returns></returns>
        public IDbConnection CreateConnection(string connectionString, string userName, string hostName)
        {
            return new SafeSqlConnection(connectionString);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public IDbConnectionHelper CreateConnectionHelper(IDbConnection connection)
        {
            var safeSqlConnection = (SafeSqlConnection)connection;
            var sqlConnection = (SqlConnection)safeSqlConnection.Connection;
            return new SqlConnectionFactory(sqlConnection, connection);
        }
    }
}