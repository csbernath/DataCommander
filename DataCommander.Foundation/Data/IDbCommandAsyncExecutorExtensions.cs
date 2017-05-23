using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace DataCommander.Foundation.Data
{
    public static class IDbCommandAsyncExecutorExtensions
    {
        public static async Task ExecuteAsync(this IDbCommandAsyncExecutor executor, IEnumerable<ExecuteCommandAsyncRequest> requests,
            CancellationToken cancellationToken)
        {
            await executor.ExecuteAsync(async connection =>
            {
                foreach (var request in requests)
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.Initialize(request.InitializeCommandRequest);
                        await request.Execute(command);
                    }
                }
            }, cancellationToken);
        }

        public static async Task ExecuteAsync(this IDbCommandAsyncExecutor executor, CreateCommandRequest request, Func<DbCommand, Task> execute,
            CancellationToken cancellationToken)
        {
            var requests = new[]
            {
                new ExecuteCommandAsyncRequest(request, execute)
            };
            await executor.ExecuteAsync(requests, cancellationToken);
        }

        public static async Task<int> ExecuteNonQueryAsync(this IDbCommandAsyncExecutor executor, CreateCommandRequest request,
            CancellationToken cancellationToken)
        {
            var affectedRows = 0;
            await executor.ExecuteAsync(request, async dbCommand => affectedRows = await dbCommand.ExecuteNonQueryAsync(cancellationToken), cancellationToken);
            return affectedRows;
        }

        public static async Task<object> ExecuteScalarAsync(this IDbCommandAsyncExecutor executor, CreateCommandRequest command,
            CancellationToken cancellationToken)
        {
            object scalar = null;
            await executor.ExecuteAsync(command, async dbCommand => scalar = await dbCommand.ExecuteScalarAsync(cancellationToken), cancellationToken);
            return scalar;
        }

        public static async Task<List<TRow>> ExecuteReaderAsync<TRow>(this IDbCommandAsyncExecutor executor, ExecuteReaderRequest request,
            Func<IDataRecord, TRow> read)
        {
            List<TRow> rows = null;
            await executor.ExecuteReaderAsync(request, async dataReader => rows = await dataReader.ReadAsync(read, request.CancellationToken));
            return rows;
        }

        public static async Task<ExecuteReaderResponse<TRow1, TRow2>> ExecuteReaderAsync<TRow1, TRow2>(this IDbCommandAsyncExecutor executor,
            ExecuteReaderRequest request, Func<IDataRecord, TRow1> read1, Func<IDataRecord, TRow2> read2)
        {
            ExecuteReaderResponse<TRow1, TRow2> response = null;
            await executor.ExecuteReaderAsync(request, async dataReader => response = await dataReader.ReadAsync(read1, read2, request.CancellationToken));
            return response;
        }

        public static async Task<ExecuteReaderResponse<TRow1, TRow2, TRow3>> ExecuteReaderAsync<TRow1, TRow2, TRow3>(this IDbCommandAsyncExecutor executor,
            ExecuteReaderRequest request, Func<IDataRecord, TRow1> read1, Func<IDataRecord, TRow2> read2, Func<IDataRecord, TRow3> read3)
        {
            ExecuteReaderResponse<TRow1, TRow2, TRow3> response = null;
            await executor.ExecuteReaderAsync(request,
                async dataReader => response = await dataReader.ReadAsync(read1, read2, read3, request.CancellationToken));
            return response;
        }

        private static async Task ExecuteReaderAsync(this IDbCommandAsyncExecutor executor, ExecuteReaderRequest request, Func<DbDataReader, Task> read)
        {
            await executor.ExecuteAsync(
                request.CreateCommandRequest,
                async dbCommand =>
                {
                    using (var dataReader = await dbCommand.ExecuteReaderAsync(request.CommandBehavior, request.CancellationToken))
                        await read(dataReader);
                },
                request.CancellationToken);
        }
    }
}