using DataCommander.Api.Connection;
using Npgsql;

namespace DataCommander.Providers.PostgreSql;

public static class ConnectionFactory
{
    public static NpgsqlConnection CreateConnection(ConnectionStringAndCredential connectionStringAndCredential)
    {
        NpgsqlConnectionStringBuilder npgsqlConnectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionStringAndCredential.ConnectionString)
        {
            ApplicationName = "Data Commander",
            Pooling = false
        };

        Credential? credential = connectionStringAndCredential.Credential;
        if (credential != null)
        {
            npgsqlConnectionStringBuilder.Username = credential.UserId;
            npgsqlConnectionStringBuilder.Password = PasswordFactory.Unprotect(credential.Password.Protected);
        }

        return new NpgsqlConnection(npgsqlConnectionStringBuilder.ConnectionString);
    }
}