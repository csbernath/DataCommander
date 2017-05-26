using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Foundation.Data
{
    internal sealed class DbCommandAsyncExecutor : IDbCommandAsyncExecutor
    {
        private readonly DbConnection _connection;

        public DbCommandAsyncExecutor(DbConnection connection)
        {
            _connection = connection;
        }

        public void Execute(Action<IDbConnection> execute)
        {
            execute(_connection);
        }

        public async Task ExecuteAsync(Func<DbConnection, Task> execute, CancellationToken cancellationToken)
        {
            await execute(_connection);
        }
    }
}