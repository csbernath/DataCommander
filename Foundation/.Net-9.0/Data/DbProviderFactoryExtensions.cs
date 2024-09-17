using System;
using System.Data;
using System.Data.Common;
using System.Globalization;

namespace Foundation.Data;

public static class DbProviderFactoryExtensions
{
    public static DataTable ExecuteDataTable(this DbProviderFactory factory, DbConnection connection, string commandText)
    {
        ArgumentNullException.ThrowIfNull(factory);
        ArgumentNullException.ThrowIfNull(connection);

        DbCommand command = connection.CreateCommand();
        command.CommandText = commandText;
        DbDataAdapter adapter = factory.CreateDataAdapter();
        adapter!.SelectCommand = command;
        DataTable table = new DataTable
        {
            Locale = CultureInfo.InvariantCulture
        };
        adapter.Fill(table);
        return table;
    }

    public static DataTable ExecuteDataTable(this DbProviderFactory dbProviderFactory, string connectionString, string commandText)
    {
        using DbConnection connection = dbProviderFactory.CreateConnection();
        connection!.ConnectionString = connectionString;
        connection.Open();
        return ExecuteDataTable(dbProviderFactory, connection, commandText);
    }

    public static object ExecuteScalar(this DbProviderFactory dbProviderFactory, string connectionString, CreateCommandRequest request)
    {
        using DbConnection connection = dbProviderFactory.CreateConnection();
        connection!.ConnectionString = connectionString;
        connection.Open();
        IDbCommandExecutor executor = connection.CreateCommandExecutor();
        return executor.ExecuteScalar(request);
    }

    public static void ExecuteTransaction(this DbProviderFactory dbProviderFactory, string connectionString, Action<IDbTransaction> action)
    {
        using (DbConnection connection = dbProviderFactory.CreateConnection())
        {
            connection.ConnectionString = connectionString;
            connection.Open();
            using (DbTransaction transaction = connection.BeginTransaction())
            {
                action(transaction);
                transaction.Commit();
            }
        }
    }
}