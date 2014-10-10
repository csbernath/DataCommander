namespace DataCommander.Foundation.Data
{
    using System.Data;
    using System.Diagnostics.Contracts;

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
        public static IDbConnection ToLoggedDbConnection( this IDbConnection connection )
        {
            Contract.Requires( connection != null );

            var loggedDbConnection = new LoggedDbConnection( connection );
            var logger = new DbConnectionLogger( loggedDbConnection );
            return loggedDbConnection;
        }
    }
}