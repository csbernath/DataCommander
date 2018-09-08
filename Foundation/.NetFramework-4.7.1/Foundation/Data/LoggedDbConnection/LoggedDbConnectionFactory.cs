using System.Data;
using Foundation.Assertions;

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
            Assert.IsNotNull(connection);
            var loggedDbConnection = new LoggedDbConnection(connection);
            var logger = new DbConnectionLogger(loggedDbConnection);
            return loggedDbConnection;
        }
    }
}