using System;
using System.Collections.Generic;
using System.Data;
using Foundation.Assertions;
using Foundation.Collections.ReadOnly;
using Foundation.Linq;

namespace Foundation.Data;

public static class IDbCommandExecutorExtensions
{
    private static void Execute(this IDbCommandExecutor executor, IEnumerable<ExecuteCommandRequest> requests)
    {
        Assert.IsNotNull(executor);

        executor.Execute(connection =>
        {
            foreach (var request in requests)
                using (var command = connection.CreateCommand(request.CreateCommandRequest))
                    request.Execute(command);
        });
    }

    public static void Execute(this IDbCommandExecutor executor, CreateCommandRequest request, Action<IDbCommand> execute)
    {
        Assert.IsNotNull(executor);
        var requests = new ExecuteCommandRequest(request, execute).ItemToArray();
        executor.Execute(requests);
    }

    public static int ExecuteNonQuery(this IDbCommandExecutor executor, CreateCommandRequest request)
    {
        Assert.IsNotNull(executor);
        var affectedRows = 0;
        executor.Execute(request, command => affectedRows = command.ExecuteNonQuery());
        return affectedRows;
    }

    public static object ExecuteScalar(this IDbCommandExecutor executor, CreateCommandRequest request)
    {
        Assert.IsNotNull(executor);
        object scalar = null;
        executor.Execute(request, command => scalar = command.ExecuteScalar());
        return scalar;
    }

    public static void ExecuteReader(this IDbCommandExecutor executor, ExecuteReaderRequest request, Action<IDataReader> readResults)
    {
        Assert.IsNotNull(executor);
        Assert.IsNotNull(request);

        executor.Execute(request.CreateCommandRequest, command =>
        {
            using (var dataReader = command.ExecuteReader(request.CommandBehavior))
                readResults(dataReader);
        });
    }

    public static ReadOnlySegmentLinkedList<T> ExecuteReader<T>(this IDbCommandExecutor executor, ExecuteReaderRequest request, int segmentLength,
        Func<IDataRecord, T> readRecord)
    {
        Assert.IsNotNull(executor);
        ReadOnlySegmentLinkedList<T> rows = null;
        executor.ExecuteReader(request, dataReader => rows = dataReader.ReadResult(segmentLength, readRecord));
        return rows;
    }

    public static DataTable ExecuteDataTable(this IDbCommandExecutor executor, ExecuteReaderRequest request)
    {
        Assert.IsNotNull(executor);
        DataTable dataTable = null;
        executor.Execute(request.CreateCommandRequest, command => { dataTable = command.ExecuteDataTable(request.CancellationToken); });
        return dataTable;
    }

    public static DataSet ExecuteDataSet(this IDbCommandExecutor executor, ExecuteReaderRequest request)
    {
        Assert.IsNotNull(executor);
        Assert.IsNotNull(request);
        DataSet dataSet = null;
        executor.Execute(request.CreateCommandRequest, command => { dataSet = command.ExecuteDataSet(request.CancellationToken); });
        return dataSet;
    }
}