using DataCommander.Api.Connection;
using Microsoft.Data.SqlClient;

namespace DataCommander.Providers.SqlServer;

public static class ConnectionFactory
{
    public static SqlConnection CreateConnection(ConnectionStringAndCredential connectionStringAndCredential)
    {
        var connectionString = CreateCustomizedConnectionString(connectionStringAndCredential.ConnectionString);
        var sqlCredential = CreateSqlCredential(connectionStringAndCredential.Credential);
        var sqlConnection = new SqlConnection(connectionString, sqlCredential);
        return sqlConnection;
    }

    private static string CreateCustomizedConnectionString(string connectionString)
    {
        var sqlConnectionStringBuilder = new SqlConnectionStringBuilder(connectionString)
        {
            ApplicationName = "Data Commander",
            Pooling = true,
            MaxPoolSize = 10,
            CommandTimeout = 8,
            ConnectTimeout = 5
        };
        return sqlConnectionStringBuilder.ConnectionString;
    }

    private static SqlCredential? CreateSqlCredential(Credential? credential)
    {
        SqlCredential? sqlCredential = null;
        if (credential != null)
        {
            var password = credential.Password.SecureString;
            sqlCredential = new SqlCredential(credential.UserId, password);
        }

        return sqlCredential;
    }
}