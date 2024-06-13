using System;
using DataCommander.Api.Connection;
using Npgsql;

namespace DataCommander.Providers.PostgreSql;

public static class ConnectionFactory
{
    public static NpgsqlConnection CreateConnection(ConnectionStringAndCredential connectionStringAndCredential)
    {
        throw new NotImplementedException();
        // SqlCredential? sqlCredential = null;
        // var credential = connectionStringAndCredential.Credential;
        // if (credential != null)
        // {
        //     var password = credential.Password.SecureString;
        //     sqlCredential = new SqlCredential(credential.UserId, password);
        // }
        //
        // return new NpgsqlConnection(connectionStringAndCredential.ConnectionString, sqlCredential);
    }
}