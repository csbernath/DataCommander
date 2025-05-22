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
            MaxPoolSize = 10
        };
        SqlCredential? sqlCredential = null;
        var credential = connectionStringAndCredential.Credential;
        if (credential != null)
        {
            var password = credential.Password.SecureString;
            sqlCredential = new SqlCredential(credential.UserId, password);
        }

        return new SqlConnection(sqlConnectionStringBuilder.ConnectionString, sqlCredential);
    }
}