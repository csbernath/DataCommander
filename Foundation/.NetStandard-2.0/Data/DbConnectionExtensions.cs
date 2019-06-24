using System.Data.Common;
using Foundation.Assertions;

namespace Foundation.Data
{
    public static class DbConnectionExtensions
    {
        public static DbCommand CreateCommand(this DbConnection connection, CreateCommandRequest request)
        {
            Assert.IsNotNull(connection);
            var command = connection.CreateCommand();
            command.Initialize(request);
            return command;
        }

        public static IDbCommandAsyncExecutor CreateCommandAsyncExecutor(this DbConnection connection) => new DbCommandAsyncExecutor(connection);
    }
}