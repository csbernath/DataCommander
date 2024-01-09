using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Foundation.Data;

internal sealed class DbCommandAsyncExecutor(DbConnection connection) : IDbCommandAsyncExecutor
{
    public void Execute(Action<IDbConnection> execute) => execute(connection);
    public Task ExecuteAsync(Func<DbConnection, Task> execute, CancellationToken cancellationToken) => execute(connection);
}