using DataCommander.Api.Connection;
using Microsoft.Data.SqlClient;

namespace DataCommander.Providers.SqlServer;

public static class ConnectionFactory
{
    public static SqlConnection CreateConnection(ConnectionStringAndCredential connectionStringAndCredential)
    {
        SqlCredential? sqlCredential = null;
        Credential? credential = connectionStringAndCredential.Credential;
        if (credential != null)
        {
            System.Security.SecureString password = credential.Password.SecureString;
            sqlCredential = new SqlCredential(credential.UserId, password);
        }

        return new SqlConnection(connectionStringAndCredential.ConnectionString, sqlCredential);
    }
}