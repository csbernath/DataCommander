using System.Data;

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
#if CONTRACTS_FULL
            FoundationContract.Requires(connection != null);
#endif

            var loggedDbConnection = new LoggedDbConnection(connection);
            var logger = new DbConnectionLogger(loggedDbConnection);
            return loggedDbConnection;
        }
    }
}