using System;
using System.Data;

namespace Foundation.Data.LoggedDbConnection;

public static class LoggedDbConnectionFactory
{
    public static IDbConnection ToLoggedDbConnection(this IDbConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);
        LoggedDbConnection loggedDbConnection = new LoggedDbConnection(connection);
        DbConnectionLogger logger = new DbConnectionLogger(loggedDbConnection);
        return loggedDbConnection;
    }
}