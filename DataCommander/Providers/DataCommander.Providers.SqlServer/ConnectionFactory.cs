using DataCommander.Api.Connection;
using Microsoft.Data.SqlClient;

namespace DataCommander.Providers.SqlServer;

public static class ConnectionFactory
{
    public static SqlConnection CreateConnection(ConnectionStringAndCredential connectionStringAndCredential)
    {
        var sqlConnectionStringBuilder = new SqlConnectionStringBuilder(connectionStringAndCredential.ConnectionString)
        {
            ApplicationName = "Data Commander",
            Pooling = true,
            MaxPoolSize = 10,
            CommandTimeout = 8,
            ConnectTimeout = 5
        };
        SqlCredential? sqlCredential = null;
        var credential = connectionStringAndCredential.Credential;
        if (credential != null)
        {
            var password = credential.Password.SecureString;
            sqlCredential = new SqlCredential(credential.UserId, password);
        }

        var sqlConnection = new SqlConnection(sqlConnectionStringBuilder.ConnectionString, sqlCredential);
        return sqlConnection;
    }
}