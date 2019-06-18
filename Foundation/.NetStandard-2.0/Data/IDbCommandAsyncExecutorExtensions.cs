using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Foundation.Data
{
    public static class IDbCommandAsyncExecutorExtensions
    {
        private static Task ExecuteAsync(this IDbCommandAsyncExecutor executor, IEnumerable<ExecuteCommandAsyncRequest> requests,
            CancellationToken cancellationToken)
        {
            return executor.ExecuteAsync(async connection =>
            {
                foreach (var request in requests)
                    using (var command = connection.CreateCommand(request.CreateCommandRequest))
                        await request.Execute(command);
            }, cancellationToken);
        }

        public static Task ExecuteAsync(this IDbCommandAsyncExecutor executor, ExecuteNonReaderRequest request, Func<DbCommand, Task> execute)
        {
            var requests = new[]
            {
                new ExecuteCommandAsyncRequest(request.CreateCommandRequest, execute)
            };
            return executor.ExecuteAsync(requests, request.CancellationToken);
        }

        public static async Task<int> ExecuteNonQueryAsync(this IDbCommandAsyncExecutor executor, ExecuteNonReaderRequest request)
        {
            var affectedRows = 0;
            await executor.ExecuteAsync(
                request,
                async command => affectedRows = await command.ExecuteNonQueryAsync(request.CancellationToken));
            return affectedRows;
        }

        public static async Task<object> ExecuteScalarAsync(this IDbCommandAsyncExecutor executor, ExecuteNonReaderRequest request)
        {
            object scalar = null;
            await executor.ExecuteAsync(
                request,
                async command => scalar = await command.ExecuteScalarAsync(request.CancellationToken));
            return scalar;
        }

        public static Task ExecuteReaderAsync(this IDbCommandAsyncExecutor executor, ExecuteReaderRequest request, Func<DbDataReader, Task> read)
        {
            return executor.ExecuteAsync(
                new ExecuteNonReaderRequest(request.CreateCommandRequest, request.CancellationToken),
                async command =>
                {
                    using (var dataReader = await command.ExecuteReaderAsync(request.CommandBehavior, request.CancellationToken))
                        await read(dataReader);
                });
        }

        public static async Task<List<T>> ExecuteReaderAsync<T>(this IDbCommandAsyncExecutor executor, ExecuteReaderRequest request, Func<IDataRecord, T> read)
        {
            List<T> records = null;
            await executor.ExecuteReaderAsync(
                request,
                async dataReader => records = await dataReader.ReadResultAsync(read, request.CancellationToken));
            return records;
        }
    }
}