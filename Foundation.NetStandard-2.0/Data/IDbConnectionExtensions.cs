using System.Data;

namespace Foundation.Data
{
    public static class IDbConnectionExtensions
    {
        public static IDbCommandExecutor CreateCommandExecutor(this IDbConnection connection)
        {
            return new DbCommandExecutor(connection);
        }

        public static IDbCommand CreateCommand(this IDbConnection connection, CreateCommandRequest request)
        {
            var command = connection.CreateCommand();
            command.Initialize(request);
            return command;
        }
    }
}