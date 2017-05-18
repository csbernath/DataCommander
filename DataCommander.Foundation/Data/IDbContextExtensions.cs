namespace DataCommander.Foundation.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Threading;
    using System.Threading.Tasks;
    using DataCommander.Foundation.Orm;

    public static class IDbContextExtensions
    {
        public static async Task ExecuteCommandsAsync(this IDbContext context, IEnumerable<ExecuteCommandRequest> requests, CancellationToken cancellationToken)
        {
            await context.ExecuteAsync(async connection =>
            {
                foreach (var request in requests)
                {
                    using (var command = CreateCommand(connection, request.CreateCommandRequest))
                    {
                        await request.Execute(command);
                    }
                }
            }, cancellationToken);
        }

        public static async Task ExecuteCommandAsync(this IDbContext context, CreateCommandRequest command, Func<DbCommand, Task> execute,
            CancellationToken cancellationToken)
        {
            var commands = new[]
            {
                new ExecuteCommandRequest(command, execute)
            };

            await context.ExecuteCommandsAsync(commands, cancellationToken);
        }

        public static async Task<int> ExecuteNonQueryAsync(this IDbContext context, CreateCommandRequest command, CancellationToken cancellationToken)
        {
            var affectedRows = 0;

            await context.ExecuteCommandAsync(
                command,
                async dbCommand => affectedRows = await dbCommand.ExecuteNonQueryAsync(cancellationToken),
                cancellationToken);

            return affectedRows;
        }

        public static async Task<object> ExecuteScalarAsync(this IDbContext context, CreateCommandRequest command, CancellationToken cancellationToken)
        {
            object scalar = null;

            await context.ExecuteCommandAsync(
                command,
                async dbCommand => scalar = await dbCommand.ExecuteScalarAsync(cancellationToken),
                cancellationToken);

            return scalar;
        }

        public static async Task<ExecuteReaderResponse<TRow>> ExecuteReaderAsync<TRow>(this IDbContext context, ExecuteReaderRequest request,
            Func<IDataRecord, TRow> read)
        {
            ExecuteReaderResponse<TRow> response = null;

            await context.ExecuteReaderAsync(
                request,
                async dataReader => response = await dataReader.ReadAsync(read, request.CancellationToken));

            return response;
        }

        public static async Task<ExecuteReaderResponse<TRow1, TRow2>> ExecuteReaderAsync<TRow1, TRow2>(this IDbContext context, ExecuteReaderRequest request,
            Func<IDataRecord, TRow1> read1, Func<IDataRecord, TRow2> read2)
        {
            ExecuteReaderResponse<TRow1, TRow2> response = null;

            await context.ExecuteReaderAsync(
                request,
                async dataReader => response = await dataReader.ReadAsync(read1, read2, request.CancellationToken));

            return response;
        }

        public static async Task<ExecuteReaderResponse<TRow1, TRow2, TRow3>> ExecuteReaderAsync<TRow1, TRow2, TRow3>(this IDbContext context,
            ExecuteReaderRequest request,
            Func<IDataRecord, TRow1> read1, Func<IDataRecord, TRow2> read2, Func<IDataRecord, TRow3> read3)
        {
            ExecuteReaderResponse<TRow1, TRow2, TRow3> response = null;

            await context.ExecuteReaderAsync(
                request,
                async dataReader => response = await dataReader.ReadAsync(read1, read2, read3, request.CancellationToken));

            return response;
        }

        private static DbCommand CreateCommand(DbConnection connection, CreateCommandRequest ormCommand)
        {
            var command = connection.CreateCommand();
            command.CommandType = ormCommand.CommandType;
            command.CommandText = ormCommand.CommandText;
            command.CommandTimeout = ormCommand.CommandTimeout;

            if (ormCommand.Parameters != null)
                command.Parameters.AddRange(ormCommand.Parameters);

            return command;
        }

        private static async Task ExecuteReaderAsync(this IDbContext context, ExecuteReaderRequest request, Func<DbDataReader, Task> read)
        {
            await context.ExecuteCommandAsync(
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