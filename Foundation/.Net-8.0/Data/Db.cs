using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Foundation.Collections.ReadOnly;

namespace Foundation.Data;

public static class Db
{
    public static ReadOnlySegmentLinkedList<T> ExecuteReader<T>(
        Func<DbConnection> createConnection,
        ExecuteReaderRequest request,
        int segmentLength,
        Func<IDataRecord, T> readRecord)
    {
        ReadOnlySegmentLinkedList<T> rows = null;
        ExecuteReader(createConnection, request, dataReader => rows = dataReader.ReadResult(segmentLength, readRecord));
        return rows;
    }

    public static void ExecuteReader(
        Func<DbConnection> createConnection,
        ExecuteReaderRequest request,
        Action<IDataReader> readResults)
    {
        using var connection = createConnection();
        connection.Open();
        var executor = connection.CreateCommandExecutor();
        executor.ExecuteReader(request, readResults);
    }

    public static async Task<ReadOnlySegmentLinkedList<T>> ExecuteReaderAsync<T>(
        Func<DbConnection> createConnection,
        ExecuteReaderRequest request,
        int segmentLength,
        Func<IDataRecord, T> readRecord,
        CancellationToken cancellationToken)
    {
        ReadOnlySegmentLinkedList<T> rows = null;
        await ExecuteReaderAsync(
            createConnection,
            request,
            async dataReader => rows = await dataReader.ReadResultAsync(segmentLength, readRecord, cancellationToken));
        return rows;
    }

    public static async Task ExecuteReaderAsync(
        Func<DbConnection> createConnection,
        ExecuteReaderRequest executeReaderRequest,
        Func<DbDataReader, Task> readResults)
    {
        await using (var connection = createConnection())
        {
            await connection.OpenAsync(executeReaderRequest.CancellationToken);
            var executor = connection.CreateCommandAsyncExecutor();
            await executor.ExecuteReaderAsync(executeReaderRequest, readResults);
        }
    }

    public static async Task<object> ExecuteScalarAsync(
        Func<DbConnection> createConnection,
        ExecuteNonReaderRequest executeNonReaderRequest,
        CancellationToken cancellationToken)
    {
        object scalar;
        await using (var connection = createConnection())
        {
            await connection.OpenAsync(cancellationToken);
            var executor = connection.CreateCommandAsyncExecutor();
            scalar = await executor.ExecuteScalarAsync(executeNonReaderRequest);
        }

        return scalar;
    }
}