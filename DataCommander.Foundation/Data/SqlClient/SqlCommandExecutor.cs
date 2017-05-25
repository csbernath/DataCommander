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
        private static readonly ILog Log = LogFactory.Instance.GetCurrentTypeLog();
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

        public async Task ExecuteAsync(Action<DbConnection> execute, CancellationToken cancellationToken)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                Log.Trace("connection.OpenAsync...");
                await connection.OpenAsync(cancellationToken);
                Log.Trace($"connection.OpenAsync. {connection.State}");
                await Task.Run(() => execute(connection), cancellationToken);
            }
        }
    }
}