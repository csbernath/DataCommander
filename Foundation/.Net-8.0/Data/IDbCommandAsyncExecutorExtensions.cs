﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Foundation.Collections.ReadOnly;

namespace Foundation.Data;

public static class IDbCommandAsyncExecutorExtensions
{
    private static Task ExecuteAsync(this IDbCommandAsyncExecutor executor, IEnumerable<ExecuteCommandAsyncRequest> requests,
        CancellationToken cancellationToken)
    {
        return executor.ExecuteAsync(
            async (connection, _) =>
            {
                foreach (var request in requests)
                    using (var command = connection.CreateCommand(request.CreateCommandRequest))
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
        var requests = new[]
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
        var affectedRows = 0;
        await executor.ExecuteAsync(
            async (connection, _) =>
            {
                await using (var command = connection.CreateCommand(createCommandRequest))
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
                scalar = await executor.ExecuteScalarAsync(createCommandRequest, cancellationToken);
            },
            cancellationToken);
        return scalar;
    }

    public static Task ExecuteReaderAsync(this IDbCommandAsyncExecutor executor, ExecuteReaderRequest request, Func<DbDataReader, Task> readResults)
    {
        return executor.ExecuteAsync(
            async (connection, _) =>
            {
                await using (var command = connection.CreateCommand(request.CreateCommandRequest))
                {
                    await using (var dataReader = await command.ExecuteReaderAsync(request.CommandBehavior, request.CancellationToken))
                        await readResults(dataReader);
                }
            }
            ,request.CancellationToken);
    }

    public static async Task<ReadOnlySegmentLinkedList<T>> ExecuteReaderAsync<T>(this IDbCommandAsyncExecutor executor, ExecuteReaderRequest request,
        int segmentLength, Func<IDataRecord, T> read)
    {
        ReadOnlySegmentLinkedList<T> records = null;
        await executor.ExecuteReaderAsync(
            request,
            async dataReader => records = await dataReader.ReadResultAsync(segmentLength, read, request.CancellationToken));
        return records;
    }
}