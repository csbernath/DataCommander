using System;
using System.Data;
using System.Data.Common;
using System.Globalization;

namespace Foundation.Data;

public static class DbProviderFactoryExtensions
{
    extension(DbProviderFactory dbProviderFactory)
    {
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
            using var connection = dbProviderFactory.CreateConnection();
            connection!.ConnectionString = connectionString;
            connection.Open();
            return ExecuteDataTable(dbProviderFactory, connection, commandText);
        }

        public object? ExecuteScalar(string connectionString, CreateCommandRequest request)
        {
            using var connection = dbProviderFactory.CreateConnection();
            connection!.ConnectionString = connectionString;
            connection.Open();
            var executor = connection.CreateCommandExecutor();
            return executor.ExecuteScalar(request);
        }

        public void ExecuteTransaction(string connectionString, Action<IDbTransaction> action)
        {
            using var connection = dbProviderFactory.CreateConnection()!;
            connection.ConnectionString = connectionString;
            connection.Open();
            using var transaction = connection.BeginTransaction();
            action(transaction);
            transaction.Commit();
        }
    }
}