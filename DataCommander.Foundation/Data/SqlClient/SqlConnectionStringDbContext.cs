namespace DataCommander.Foundation.Data.SqlClient
{
    using System;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class SqlConnectionStringDbContext : IDbContext
    {
        private readonly string _connectionString;

        public SqlConnectionStringDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task ExecuteAsync(Action<DbConnection> execute, CancellationToken cancellationToken)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                execute(connection);
            }
        }
    }
}