namespace DataCommander.Foundation.Data
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class SqlCommandExecutor : IDbCommandExecutor
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

        public async Task ExecuteAsync(Action<DbConnection> execute, CancellationToken cancellationToken)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                await Task.Run(() => execute(connection), cancellationToken);
            }
        }
    }
}