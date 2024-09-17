using System.Data;
using Foundation.Threading;
using Microsoft.Data.SqlClient;

namespace Foundation.Data.SqlClient;

public class SafeSqlConnectionFactory : IDbConnectionFactory
{
    public WorkerThread Thread => null;
    public IDbConnection CreateConnection(string connectionString, string userName, string hostName) => new SafeSqlConnection(connectionString);

    public IDbConnectionHelper CreateConnectionHelper(IDbConnection connection)
    {
        SafeSqlConnection safeSqlConnection = (SafeSqlConnection)connection;
        SqlConnection sqlConnection = (SqlConnection)safeSqlConnection.Connection;
        return new SqlConnectionFactory(sqlConnection, connection);
    }
}