using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Foundation.Collections.ReadOnly;

namespace Foundation.Data;

public static class IDbCommandAsyncExecutorExtensions
{
    private static Task ExecuteAsync(
        this IDbCommandAsyncExecutor executor,
        IEnumerable<ExecuteCommandAsyncRequest> requests,
        CancellationToken cancellationToken)
    {
        return executor.ExecuteAsync(
            async (connection, _) =>
            {
                foreach (ExecuteCommandAsyncRequest request in requests)
                    using (DbCommand command = connection.CreateCommand(request.CreateCommandRequest))
                        await request.Execute(command);
            },
            cancellationToken);
    }

    public static Task ExecuteAsync(
        this IDbCommandAsyncExecutor executor,
        CreateCommandRequest createCommandRequest,
        Func<DbCommand, Task> executeCommand,
        CancellationToken cancellationToken)
    {
        ExecuteCommandAsyncRequest[] requests = new[]
        {
            new ExecuteCommandAsyncRequest(createCommandRequest, executeCommand)
        };
        return executor.ExecuteAsync(requests, cancellationToken);
    }

    public static async Task<int> ExecuteNonQueryAsync(
        this IDbCommandAsyncExecutor executor,
        CreateCommandRequest createCommandRequest,
        CancellationToken cancellationToken)
    {
        int affectedRows = 0;
        await executor.ExecuteAsync(
            async (connection, _) =>
            {
                await using (DbCommand command = connection.CreateCommand(createCommandRequest))
                    affectedRows = await command.ExecuteNonQueryAsync(cancellationToken);
            },
            cancellationToken);
        return affectedRows;
    }

    public static async Task<object> ExecuteScalarAsync(
        this IDbCommandAsyncExecutor executor,
        CreateCommandRequest createCommandRequest,
        CancellationToken cancellationToken)
    {
        object scalar = null;
        await executor.ExecuteAsync(
            async (connection, _) =>
            {
                await using (DbCommand command = connection.CreateCommand(createCommandRequest))
                    scalar = await command.ExecuteScalarAsync(cancellationToken);
            },
            cancellationToken);
        return scalar;
    }

    public static Task ExecuteReaderAsync(
        this IDbCommandAsyncExecutor executor,
        ExecuteReaderRequest executeReaderRequest,
        Func<DbDataReader, CancellationToken, Task> readResults,
        CancellationToken cancellationToken)
    {
        return executor.ExecuteAsync(
            async (connection, _) =>
            {
                await using (DbCommand command = connection.CreateCommand(executeReaderRequest.CreateCommandRequest))
                {
                    await using (DbDataReader dataReader =
                                 await command.ExecuteReaderAsync(executeReaderRequest.CommandBehavior,
                                     cancellationToken))
                        await readResults(dataReader, cancellationToken);
                }
            },
            cancellationToken);
    }

    public static async Task<ReadOnlySegmentLinkedList<T>> ExecuteReaderAsync<T>(
        this IDbCommandAsyncExecutor executor,
        ExecuteReaderRequest executeReaderRequest,
        int segmentLength,
        Func<IDataRecord, T> readDataRecord,
        CancellationToken cancellationToken)
    {
        ReadOnlySegmentLinkedList<T> records = null;
        await executor.ExecuteReaderAsync(
            executeReaderRequest,
            async (dataReader, _) => records =
                await dataReader.ReadResultAsync(segmentLength, readDataRecord, cancellationToken),
            cancellationToken);
        return records;
    }
}