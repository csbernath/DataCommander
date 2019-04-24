using System.Data;
using Foundation.Assertions;

namespace Foundation.Data.LoggedDbConnection
{
    public static class LoggedDbConnectionFactory
    {
        public static IDbConnection ToLoggedDbConnection(this IDbConnection connection)
        {
            Assert.IsNotNull(connection);
            var loggedDbConnection = new LoggedDbConnection(connection);
            var logger = new DbConnectionLogger(loggedDbConnection);
            return loggedDbConnection;
        }
    }
}