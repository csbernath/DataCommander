using System.Data;

namespace Foundation.Data;

public static class IDbConnectionExtensions
{
    extension(IDbConnection connection)
    {
        public IDbCommandExecutor CreateCommandExecutor() => new DbCommandExecutor(connection);

        public IDbCommand CreateCommand(CreateCommandRequest request)
        {
            var command = connection.CreateCommand();
            command.Initialize(request);
            return command;
        }
    }
}