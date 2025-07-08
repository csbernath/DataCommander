using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using Foundation.Collections.ReadOnly;
using Foundation.Linq;

namespace Foundation.Data;

public static class IDbCommandExecutorExtensions
{
    private static void Execute(this IDbCommandExecutor executor, IEnumerable<ExecuteCommandRequest> requests)
    {
        ArgumentNullException.ThrowIfNull(executor);

        executor.Execute(connection =>
        {
            foreach (var request in requests)
                using (var command = connection.CreateCommand(request.CreateCommandRequest))
                    request.Execute(command);
        });
    }

    public static void Execute(this IDbCommandExecutor executor, CreateCommandRequest request, Action<IDbCommand> execute)
    {
        ArgumentNullException.ThrowIfNull(executor);
        ArgumentNullException.ThrowIfNull(request);
        
        var requests = new ExecuteCommandRequest(request, execute).ItemToArray();
        executor.Execute(requests);
    }

    public static int ExecuteNonQuery(this IDbCommandExecutor executor, CreateCommandRequest request)
    {
        ArgumentNullException.ThrowIfNull(executor);
        ArgumentNullException.ThrowIfNull(request);
        
        var affectedRows = 0;
        executor.Execute(request, command => affectedRows = command.ExecuteNonQuery());
        return affectedRows;
    }

    public static object? ExecuteScalar(this IDbCommandExecutor executor, CreateCommandRequest request)
    {
        ArgumentNullException.ThrowIfNull(executor);
        ArgumentNullException.ThrowIfNull(request);

        object? scalar = null;
        executor.Execute(request, command => scalar = command.ExecuteScalar());
        return scalar;
    }

    public static void ExecuteReader(this IDbCommandExecutor executor, ExecuteReaderRequest request, Action<IDataReader> readResults)
    {
        ArgumentNullException.ThrowIfNull(executor);
        ArgumentNullException.ThrowIfNull(request);

        executor.Execute(request.CreateCommandRequest, command =>
        {
            using var dataReader = command.ExecuteReader(request.CommandBehavior);
            readResults(dataReader);
        });
    }

    public static ReadOnlySegmentLinkedList<T> ExecuteReader<T>(this IDbCommandExecutor executor, ExecuteReaderRequest request, int segmentLength,
        Func<IDataRecord, T> readRecord)
    {
        ArgumentNullException.ThrowIfNull(executor);

        ReadOnlySegmentLinkedList<T>? rows = null;
        executor.ExecuteReader(request, dataReader => rows = dataReader.ReadResult(segmentLength, readRecord));
        return rows!;
    }

    public static DataTable ExecuteDataTable(this IDbCommandExecutor executor, ExecuteReaderRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(executor);

        DataTable? dataTable = null;
        executor.Execute(
            request.CreateCommandRequest,
            command => { dataTable = command.ExecuteDataTable(cancellationToken); });
        return dataTable!;
    }

    public static DataSet ExecuteDataSet(this IDbCommandExecutor executor, ExecuteReaderRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(executor);
        ArgumentNullException.ThrowIfNull(request);

        DataSet? dataSet = null;
        executor.Execute(
            request.CreateCommandRequest,
            command => { dataSet = command.ExecuteDataSet(cancellationToken); });
        return dataSet!;
    }
}