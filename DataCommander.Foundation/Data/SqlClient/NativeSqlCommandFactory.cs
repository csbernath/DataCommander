namespace DataCommander.Foundation.Data.SqlClient
{
    using System.Data;
    using System.Data.SqlClient;
    using DataCommander.Foundation.Threading;

    /// <summary>
    /// 
    /// </summary>
    public class NativeSqlCommandFactory : IDbConnectionFactory
    {
        WorkerThread IDbConnectionFactory.Thread => null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="userName"></param>
        /// <param name="hostName"></param>
        /// <returns></returns>
        public IDbConnection CreateConnection(string connectionString, string userName, string hostName)
        {
            return new SqlConnection(connectionString);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public IDbConnectionHelper CreateConnectionHelper(IDbConnection connection)
        {
            var sqlConnection = (SqlConnection)connection;
            return new SqlConnectionFactory(sqlConnection, connection);
        }
    }
}