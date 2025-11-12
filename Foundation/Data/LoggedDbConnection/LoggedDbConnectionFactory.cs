using System;
using System.Data;

namespace Foundation.Data.LoggedDbConnection;

public static class LoggedDbConnectionFactory
{
    extension(IDbConnection connection)
    {
        public IDbConnection ToLoggedDbConnection()
        {
            ArgumentNullException.ThrowIfNull(connection);
            var loggedDbConnection = new LoggedDbConnection(connection);
            var logger = new DbConnectionLogger(loggedDbConnection);
            return loggedDbConnection;
        }
    }
}