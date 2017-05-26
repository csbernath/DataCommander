using System.Data.Common;

namespace DataCommander.Foundation.Data
{
    public static class DbConnectionExtensions
    {
        public static DbCommand CreateCommand(this DbConnection connection, CreateCommandRequest request)
        {
            var command = connection.CreateCommand();
            command.Initialize(request);
            return command;
        }

        public static IDbCommandAsyncExecutor CreateCommandAsyncExecutor(this DbConnection connection)
        {
            return new DbCommandAsyncExecutor(connection);
        }
    }
}