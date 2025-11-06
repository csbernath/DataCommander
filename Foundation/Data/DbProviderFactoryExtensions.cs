using System;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Foundation.Data;

public static class DbProviderFactoryExtensions
{
    extension(DbProviderFactory dbProviderFactory)
    {
        public DbConnection CreateConnection(string connectionString)
        {
            ArgumentNullException.ThrowIfNull(dbProviderFactory);
            var connection = dbProviderFactory.CreateConnection()!;
            connection.ConnectionString = connectionString;
            return connection;
        }

        public DbConnection OpenConnection(string connectionString)
        {
            var connection = dbProviderFactory.CreateConnection(connectionString);
            connection.Open();
            return connection;
        }

        public async Task<DbConnection> OpenConnectionAsync(string connectionString, CancellationToken cancellationToken)
        {
            var connection = dbProviderFactory.CreateConnection(connectionString);
            await connection.OpenAsync(cancellationToken);
            return connection;
        }

        public DataTable ExecuteDataTable(DbConnection connection, string commandText)
        {
            ArgumentNullException.ThrowIfNull(dbProviderFactory);
            ArgumentNullException.ThrowIfNull(connection);

            var command = connection.CreateCommand();
            command.CommandText = commandText;
            var adapter = dbProviderFactory.CreateDataAdapter();
            adapter!.SelectCommand = command;
            var table = new DataTable
            {
                Locale = CultureInfo.InvariantCulture
            };
            adapter.Fill(table);
            return table;
        }

        public DataTable ExecuteDataTable(string connectionString, string commandText)
        {
            using var connection = dbProviderFactory.OpenConnection(connectionString);
            return ExecuteDataTable(dbProviderFactory, connection, commandText);
        }

        public object? ExecuteScalar(string connectionString, CreateCommandRequest request)
        {
            using var connection = dbProviderFactory.OpenConnection(connectionString);
            var executor = connection.CreateCommandExecutor();
            return executor.ExecuteScalar(request);
        }

        public void ExecuteTransaction(string connectionString, Action<IDbTransaction> action)
        {
            using var connection = dbProviderFactory.OpenConnection(connectionString);
            using var transaction = connection.BeginTransaction();
            action(transaction);
            transaction.Commit();
        }

        public async Task ExecuteTransactionAsync(string connectionString, Func<DbTransaction, CancellationToken, Task> execute, CancellationToken cancellationToken)
        {
            using var connection = await dbProviderFactory.OpenConnectionAsync(connectionString, cancellationToken);
            using var transaction = await connection.BeginTransactionAsync(cancellationToken);
            await execute(transaction, cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
    }
}