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
            int applicationId,
            string userName,
            string hostName,
            string connectionString,
            ISqlLoggedSqlCommandFilter filter )
        {
            SqlLoggedSqlConnection connection = new SqlLoggedSqlConnection( sqlLog, applicationId, userName, hostName, connectionString, filter );
            this.Initialize( connection, this );
        }

        object ISafeDbConnection.Id
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