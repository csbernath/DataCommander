namespace DataCommander.Foundation.Data
{
    using System;
    using System.Threading.Tasks;

    using System.Data.Common;
    using System.Threading;

    public sealed class DbConnectionDbContext : IDbContext
    {
        private readonly DbConnection _connection;

        public DbConnectionDbContext(DbConnection connection)
        {
            _connection = connection;
        }

        public async Task ExecuteAsync(Action<DbConnection> execute, CancellationToken cancellationToken)
        {
            await Task.Run(() => execute(_connection), cancellationToken);
        }
    }
}