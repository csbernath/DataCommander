using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Foundation.Data.SqlClient;

public sealed class SqlCommandExecutor(Func<SqlConnection> createConnection) : IDbCommandAsyncExecutor
{
    public void Execute(Action<IDbConnection> execute)
    {
        using (var connection = createConnection())
        {
            connection.Open();
            execute(connection);
        }
    }

    public async Task ExecuteAsync(Func<DbConnection, CancellationToken, Task> execute, CancellationToken cancellationToken)
    {
        await using (var connection = createConnection())
        {
            await connection.OpenAsync(cancellationToken);
            await execute(connection, cancellationToken);
        }
    }
}