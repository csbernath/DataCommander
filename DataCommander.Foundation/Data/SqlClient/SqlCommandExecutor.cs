using DataCommander.Foundation.Diagnostics.Log;

namespace DataCommander.Foundation.Data
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class SqlCommandExecutor : IDbCommandAsyncExecutor
    {
        private readonly string _connectionString;

        public SqlCommandExecutor(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Execute(Action<IDbConnection> execute)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                execute(connection);
            }
        }

        public async Task ExecuteAsync(Func<DbConnection, Task> execute, CancellationToken cancellationToken)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                await execute(connection);
            }
        }
    }
}