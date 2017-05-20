namespace DataCommander.Foundation.Data.SqlClient
{
    using System;
    using System.Data;
    using System.Threading;
    using DataCommander.Foundation.Data.SqlClient.SqlLoggedSqlConnection;

    /// <summary>
    /// 
    /// </summary>
    public class SafeLoggedSqlConnection : SafeDbConnection, ISafeDbConnection
    {
        private readonly CancellationToken _cancellationToken;
        private short _id;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlLog"></param>
        /// <param name="applicationId"></param>
        /// <param name="userName"></param>
        /// <param name="hostName"></param>
        /// <param name="connectionString"></param>
        /// <param name="filter"></param>
        /// <param name="cancellationToken"></param>
        public SafeLoggedSqlConnection(
            SqlLog.SqlLog sqlLog,
            int applicationId,
            string userName,
            string hostName,
            string connectionString,
            ISqlLoggedSqlCommandFilter filter,
            CancellationToken cancellationToken)
        {
            var connection = new SqlLoggedSqlConnection.SqlLoggedSqlConnection(sqlLog, applicationId, userName, hostName, connectionString, filter);
            this._cancellationToken = cancellationToken;

            this.Initialize(connection, this);
        }

        CancellationToken ISafeDbConnection.CancellationToken => this._cancellationToken;

        object ISafeDbConnection.Id
        {
            get
            {
                if (this._id == 0)
                {
                    this._id = SafeSqlConnection.GetId(this.Connection);
                }

                return this._id;
            }
        }

        void ISafeDbConnection.HandleException(Exception exception, TimeSpan elapsed)
        {
            SafeSqlConnection.HandleException(this.Connection, exception, elapsed, this._cancellationToken);
        }

        void ISafeDbConnection.HandleException(Exception exception, IDbCommand command)
        {
            SafeSqlConnection.HandleException(exception, command, this._cancellationToken);
        }
    }
}