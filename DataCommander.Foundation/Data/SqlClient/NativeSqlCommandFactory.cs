namespace DataCommander.Foundation.Data.SqlClient
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    using DataCommander.Foundation.Threading;

    /// <summary>
    /// 
    /// </summary>
    public class NativeSqlCommandFactory : IDbConnectionFactory
    {
        WorkerThread IDbConnectionFactory.Thread
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="userName"></param>
        /// <param name="hostName"></param>
        /// <returns></returns>
        public IDbConnection CreateConnection(String connectionString, String userName, String hostName)
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
            SqlConnection sqlConnection = (SqlConnection)connection;
            return new SqlConnectionFactory(sqlConnection, connection);
        }
    }
}