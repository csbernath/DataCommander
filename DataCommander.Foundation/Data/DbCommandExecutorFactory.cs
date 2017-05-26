using System.Data;
using System.Data.Common;

namespace Foundation.Data
{
    public static class DbCommandExecutorFactory
    {
        public static IDbCommandExecutor Create(IDbConnection connection)
        {
            return new DbCommandExecutor(connection);
        }

        public static IDbCommandAsyncExecutor Create(DbConnection connection)
        {
            return new DbCommandAsyncExecutor(connection);
        }
    }
}