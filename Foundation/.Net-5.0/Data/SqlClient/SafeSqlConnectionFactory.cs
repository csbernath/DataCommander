using System.Data;
using System.Data.SqlClient;
using Foundation.Threading;

namespace Foundation.Data.SqlClient;

public class SafeSqlConnectionFactory : IDbConnectionFactory
{
    public WorkerThread Thread => null;
    public IDbConnection CreateConnection(string connectionString, string userName, string hostName) => new SafeSqlConnection(connectionString);

    public IDbConnectionHelper CreateConnectionHelper(IDbConnection connection)
    {
        var safeSqlConnection = (SafeSqlConnection)connection;
        var sqlConnection = (SqlConnection)safeSqlConnection.Connection;
        return new SqlConnectionFactory(sqlConnection, connection);
    }
}