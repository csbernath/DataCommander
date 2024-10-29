using System;
using DataCommander.Api.Connection;
using Microsoft.Data.Sqlite;

namespace DataCommander.Providers.SQLite;

public static class ConnectionFactory
{
    [CLSCompliant(false)]
    public static SqliteConnection CreateConnection(ConnectionStringAndCredential connectionStringAndCredential)
    {
        var sqliteConnectionStringBuilder = new SqliteConnectionStringBuilder(connectionStringAndCredential.ConnectionString)
        {
            Pooling = false
        };

        var credential = connectionStringAndCredential.Credential;
        if (credential != null)
            sqliteConnectionStringBuilder.Password = PasswordFactory.Unprotect(credential.Password.Protected);

        return new SqliteConnection(sqliteConnectionStringBuilder.ConnectionString);
    }
}