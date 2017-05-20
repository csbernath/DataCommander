namespace DataCommander.Foundation.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Threading;
    using System.Threading.Tasks;

    public static class IDbCommandExecutorExtensions
    {
        public static void ExecuteCommands(this IDbCommandExecutor executor, IEnumerable<ExecuteCommandRequest> requests)
        {
            executor.Execute(connection =>
            {
                foreach (var request in requests)
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.Initialize(request.InitializeCommandRequest);
                        request.Execute(command);
                    }
                }
            });
        }

        public static async Task ExecuteCommandsAsync(this IDbCommandExecutor executor, IEnumerable<ExecuteCommandAsyncRequest> requests,
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

        public static void ExecuteCommand(this IDbCommandExecutor executor, CreateCommandRequest request, Action<IDbCommand> execute)
        {
            var requests = new[]
            {
                new ExecuteCommandRequest(request, execute)
            };

            executor.ExecuteCommands(requests);
        }

        public static async Task ExecuteCommandAsync(this IDbCommandExecutor executor, CreateCommandRequest request, Func<DbCommand, Task> execute,
            CancellationToken cancellationToken)
        {
            var requests = new[]
            {
                new ExecuteCommandAsyncRequest(request, execute)
            };

            await executor.ExecuteCommandsAsync(requests, cancellationToken);
        }

        public static int ExecuteNonQuery(this IDbCommandExecutor executor, CreateCommandRequest request)
        {
            var affectedRows = 0;
            executor.ExecuteCommand(request, command => affectedRows = command.ExecuteNonQuery());
            return affectedRows;
        }

        public static async Task<int> ExecuteNonQueryAsync(this IDbCommandExecutor executor, CreateCommandRequest request,
            CancellationToken cancellationToken)
        {
            var affectedRows = 0;

            await executor.ExecuteCommandAsync(
                request,
                async dbCommand => affectedRows = await dbCommand.ExecuteNonQueryAsync(cancellationToken),
                cancellationToken);

            return affectedRows;
        }

        public static object ExecuteScalar(this IDbCommandExecutor executor, CreateCommandRequest request)
        {
            object scalar = null;
            executor.ExecuteCommand(request, command => scalar = command.ExecuteScalar());
            return scalar;
        }

        public static async Task<object> ExecuteScalarAsync(this IDbCommandExecutor executor, CreateCommandRequest command,
            CancellationToken cancellationToken)
        {
            object scalar = null;

            await executor.ExecuteCommandAsync(
                command,
                async dbCommand => scalar = await dbCommand.ExecuteScalarAsync(cancellationToken),
                cancellationToken);

            return scalar;
        }

        public static ExecuteReaderResponse<TRow> ExecuteReader<TRow>(this IDbCommandExecutor executor, ExecuteReaderRequest request,
            Func<IDataRecord, TRow> read)
        {
            ExecuteReaderResponse<TRow> response = null;

            executor.ExecuteReader(
                request,
                dataReader => response = dataReader.Read(read));

            return response;
        }

        public static async Task<ExecuteReaderResponse<TRow>> ExecuteReaderAsync<TRow>(this IDbCommandExecutor executor, ExecuteReaderRequest request,
            Func<IDataRecord, TRow> read)
        {
            ExecuteReaderResponse<TRow> response = null;

            await executor.ExecuteReaderAsync(
                request,
                async dataReader => response = await dataReader.ReadAsync(read, request.CancellationToken));

            return response;
        }

        public static ExecuteReaderResponse<TRow1, TRow2> ExecuteReader<TRow1, TRow2>(this IDbCommandExecutor executor, ExecuteReaderRequest request,
            Func<IDataRecord, TRow1> read1, Func<IDataRecord, TRow2> read2)
        {
            ExecuteReaderResponse<TRow1, TRow2> response = null;

            executor.ExecuteReader(
                request,
                dataReader => response = dataReader.Read(read1, read2));

            return response;
        }

        public static async Task<ExecuteReaderResponse<TRow1, TRow2>> ExecuteReaderAsync<TRow1, TRow2>(this IDbCommandExecutor executor,
            ExecuteReaderRequest request, Func<IDataRecord, TRow1> read1, Func<IDataRecord, TRow2> read2)
        {
            ExecuteReaderResponse<TRow1, TRow2> response = null;

            await executor.ExecuteReaderAsync(
                request,
                async dataReader => response = await dataReader.ReadAsync(read1, read2, request.CancellationToken));

            return response;
        }

        public static ExecuteReaderResponse<TRow1, TRow2, TRow3> ExecuteReader<TRow1, TRow2, TRow3>(
            this IDbCommandExecutor executor,
            ExecuteReaderRequest request,
            Func<IDataRecord, TRow1> read1,
            Func<IDataRecord, TRow2> read2,
            Func<IDataRecord, TRow3> read3)
        {
            ExecuteReaderResponse<TRow1, TRow2, TRow3> response = null;

            executor.ExecuteReader(
                request,
                dataReader => response = dataReader.Read(read1, read2, read3));

            return response;
        }

        public static async Task<ExecuteReaderResponse<TRow1, TRow2, TRow3>> ExecuteReaderAsync<TRow1, TRow2, TRow3>(
            this IDbCommandExecutor executor,
            ExecuteReaderRequest request,
            Func<IDataRecord, TRow1> read1,
            Func<IDataRecord, TRow2> read2,
            Func<IDataRecord, TRow3> read3)
        {
            ExecuteReaderResponse<TRow1, TRow2, TRow3> response = null;

            await executor.ExecuteReaderAsync(
                request,
                async dataReader => response = await dataReader.ReadAsync(read1, read2, read3, request.CancellationToken));

            return response;
        }

        private static void ExecuteReader(this IDbCommandExecutor executor, ExecuteReaderRequest request, Action<IDataReader> read)
        {
            executor.ExecuteCommand(request.InitializeCommandRequest, command =>
            {
                using (var dataReader = command.ExecuteReader(request.CommandBehavior))
                    read(dataReader);
            });
        }

        private static async Task ExecuteReaderAsync(this IDbCommandExecutor executor, ExecuteReaderRequest request, Func<DbDataReader, Task> read)
        {
            await executor.ExecuteCommandAsync(
                request.InitializeCommandRequest,
                async dbCommand =>
                {
                    using (var dataReader = await dbCommand.ExecuteReaderAsync(request.CommandBehavior, request.CancellationToken))
                        await read(dataReader);
                },
                request.CancellationToken);
        }
    }
}