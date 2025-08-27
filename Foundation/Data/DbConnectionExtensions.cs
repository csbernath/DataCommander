using System;
using System.Data.Common;

namespace Foundation.Data;

public static class DbConnectionExtensions
{
    extension(DbConnection connection)
    {
        public DbCommand CreateCommand(CreateCommandRequest request)
        {
            ArgumentNullException.ThrowIfNull(connection);
            var command = connection.CreateCommand();
            command.Initialize(request);
            return command;
        }

        public IDbCommandAsyncExecutor CreateCommandAsyncExecutor() => new DbCommandAsyncExecutor(connection);
    }
}