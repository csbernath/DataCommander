using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace DataCommander.Foundation.Data
{
    public sealed class DbCommandExecutor : IDbCommandExecutor
    {
        private readonly DbConnection _connection;

        public DbCommandExecutor(DbConnection connection)
        {
            _connection = connection;
        }

        public void Execute(Action<IDbConnection> execute)
        {
            execute(_connection);
        }

        public async Task ExecuteAsync(Action<DbConnection> execute, CancellationToken cancellationToken)
        {
            await Task.Run(() => execute(_connection), cancellationToken);
        }
    }
}