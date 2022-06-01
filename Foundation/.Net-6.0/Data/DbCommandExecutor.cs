using System;
using System.Data;

namespace Foundation.Data;

public sealed class DbCommandExecutor : IDbCommandExecutor
{
    private readonly IDbConnection _connection;

    public DbCommandExecutor(IDbConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);
        _connection = connection;
    }

    public void Execute(Action<IDbConnection> execute)
    {
        ArgumentNullException.ThrowIfNull(execute);
        execute(_connection);
    }
}