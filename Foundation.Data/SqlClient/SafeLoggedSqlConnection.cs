using System;
using System.Data;
using System.Threading;
using Foundation.Data.SqlClient.SqlLoggedSqlConnection;

namespace Foundation.Data.SqlClient
{
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
            _cancellationToken = cancellationToken;

            Initialize(connection, this);
        }

        CancellationToken ISafeDbConnection.CancellationToken => _cancellationToken;

        object ISafeDbConnection.Id
        {
            get
            {
                if (_id == 0)
                {
                    _id = SafeSqlConnection.GetId(Connection);
                }

                return _id;
            }
        }

        void ISafeDbConnection.HandleException(Exception exception, TimeSpan elapsed)
        {
            SafeSqlConnection.HandleException(Connection, exception, elapsed, _cancellationToken);
        }

        void ISafeDbConnection.HandleException(Exception exception, IDbCommand command)
        {
            SafeSqlConnection.HandleException(exception, command, _cancellationToken);
        }
    }
}