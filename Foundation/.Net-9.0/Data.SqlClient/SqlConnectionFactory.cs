using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Foundation.Data.SqlClient;

internal sealed class SqlConnectionFactory : IDbConnectionHelper
{
    private readonly IDbConnection _connection;

    public event EventHandler? InfoMessage;

    public SqlConnectionFactory(SqlConnection sqlConnection, IDbConnection connection)
    {
        sqlConnection.InfoMessage += InfoMessageEvent;
        _connection = connection;
    }

    private void InfoMessageEvent(object? sender, SqlInfoMessageEventArgs e) => InfoMessage?.Invoke(_connection, e);
}