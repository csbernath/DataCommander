using System;
using System.Data;

namespace Foundation.Data.LoggedDbConnection;

public static class LoggedDbConnectionFactory
{
    public static IDbConnection ToLoggedDbConnection(this IDbConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);
        var loggedDbConnection = new LoggedDbConnection(connection);
        var logger = new DbConnectionLogger(loggedDbConnection);
        return loggedDbConnection;
    }
}