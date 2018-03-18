using System;
using System.Data;
using Foundation.Diagnostics.Contracts;

namespace Foundation.Data.LoggedDbConnection
{
    /// <summary>
    /// 
    /// </summary>
    public static class LoggedDbConnectionFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static IDbConnection ToLoggedDbConnection(this IDbConnection connection)
        {
            FoundationContract.Requires<ArgumentException>(connection != null);

            var loggedDbConnection = new LoggedDbConnection(connection);
            var logger = new DbConnectionLogger(loggedDbConnection);
            return loggedDbConnection;
        }
    }
}