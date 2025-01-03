﻿using System;
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

        var command = connection.CreateCommand();
        command.CommandText = commandText;
        var adapter = factory.CreateDataAdapter();
        adapter!.SelectCommand = command;
        var table = new DataTable
        {
            Locale = CultureInfo.InvariantCulture
        };
        adapter.Fill(table);
        return table;
    }

    public static DataTable ExecuteDataTable(this DbProviderFactory dbProviderFactory, string connectionString, string commandText)
    {
        using var connection = dbProviderFactory.CreateConnection();
        connection!.ConnectionString = connectionString;
        connection.Open();
        return ExecuteDataTable(dbProviderFactory, connection, commandText);
    }

    public static object? ExecuteScalar(this DbProviderFactory dbProviderFactory, string connectionString, CreateCommandRequest request)
    {
        using var connection = dbProviderFactory.CreateConnection();
        connection!.ConnectionString = connectionString;
        connection.Open();
        var executor = connection.CreateCommandExecutor();
        return executor.ExecuteScalar(request);
    }

    public static void ExecuteTransaction(this DbProviderFactory dbProviderFactory, string connectionString, Action<IDbTransaction> action)
    {
        using var connection = dbProviderFactory.CreateConnection()!;
        connection.ConnectionString = connectionString;
        connection.Open();
        using var transaction = connection.BeginTransaction();
        action(transaction);
        transaction.Commit();
    }
}