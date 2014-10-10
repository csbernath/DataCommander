namespace DataCommander.Foundation.Data.SqlClient
{
    using System;
    using System.Data;

    /// <summary>
    /// 
    /// </summary>
    public class SafeLoggedSqlConnection : SafeDbConnection, ISafeDbConnection
    {
        private Int16 id;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlLog"></param>
        /// <param name="applicationId"></param>
        /// <param name="userName"></param>
        /// <param name="hostName"></param>
        /// <param name="connectionString"></param>
        /// <param name="filter"></param>
        public SafeLoggedSqlConnection(
            SqlLog sqlLog,
            Int32 applicationId,
            String userName,
            String hostName,
            String connectionString,
            ISqlLoggedSqlCommandFilter filter )
        {
            SqlLoggedSqlConnection connection = new SqlLoggedSqlConnection( sqlLog, applicationId, userName, hostName, connectionString, filter );
            this.Initialize( connection, this );
        }

        Object ISafeDbConnection.Id
        {
            get
            {
                if (this.id == 0)
                {
                    this.id = SafeSqlConnection.GetId( this.Connection );
                }

                return this.id;
            }
        }

        void ISafeDbConnection.HandleException( Exception exception, TimeSpan elapsed )
        {
            SafeSqlConnection.HandleException( this.Connection, exception, elapsed );
        }

        void ISafeDbConnection.HandleException( Exception exception, IDbCommand command )
        {
            SafeSqlConnection.HandleException( exception, command );
        }
    }
}