using System.Data;
using System.Data.SqlClient;
using Foundation.Threading;

namespace Foundation.Data.SqlClient
{
    public class NativeSqlCommandFactory : IDbConnectionFactory
    {
        WorkerThread IDbConnectionFactory.Thread => null;

        public IDbConnection CreateConnection(string connectionString, string userName, string hostName) => new SqlConnection(connectionString);

        public IDbConnectionHelper CreateConnectionHelper(IDbConnection connection)
        {
            var sqlConnection = (SqlConnection)connection;
            return new SqlConnectionFactory(sqlConnection, connection);
        }
    }
}