using System.Data.OleDb;
using DataCommander.Api.Connection;

namespace DataCommander.Providers.OleDb;

public static class ConnectionFactory
{
    public static OleDbConnection CreateConnection(ConnectionStringAndCredential connectionStringAndCredential)
    {
        return new OleDbConnection(connectionStringAndCredential.ConnectionString);
    }
}