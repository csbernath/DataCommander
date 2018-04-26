using System.Data;
using System.Data.Common;

namespace Foundation.Data
{
    public static class DbCommandExecutorFactory
    {
        public static IDbCommandExecutor Create(IDbConnection connection) => new DbCommandExecutor(connection);
        public static IDbCommandAsyncExecutor Create(DbConnection connection) => new DbCommandAsyncExecutor(connection);
    }
}