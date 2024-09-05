using System.Data.SQLite;
using DataCommander.Api.Connection;

namespace DataCommander.Providers.SQLite;

public static class ConnectionFactory
{
    public static SQLiteConnection CreateConnection(ConnectionStringAndCredential connectionStringAndCredential)
    {
        var sqliteConnectionStringBuilder = new SQLiteConnectionStringBuilder(connectionStringAndCredential.ConnectionString)
        {
            Pooling = false
        };

        var credential = connectionStringAndCredential.Credential;
        if (credential != null)
            sqliteConnectionStringBuilder.Password = PasswordFactory.Unprotect(credential.Password.Protected);

        return new SQLiteConnection(sqliteConnectionStringBuilder.ConnectionString);
    }
}