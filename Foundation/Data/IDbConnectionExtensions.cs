using System.Data;

namespace Foundation.Data
{
    public static class IDbConnectionExtensions
    {
        public static IDbCommandExecutor CreateCommandExecutor(this IDbConnection connection)
        {
            return new DbCommandExecutor(connection);
        }
    }
}