using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Foundation.Assertions;

namespace Foundation.Data.SqlClient.SqlLoggedSqlConnection
{
    /// <summary>
    /// Logged SqlConnection class.
    /// </summary>
    public sealed class SqlLoggedSqlConnection : IDbConnection
    {
        private readonly int _applicationId;
        private int _connectionNo;
        private readonly SqlLog.SqlLog _sqlLog;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlLog"></param>
        /// <param name="applicationId"></param>
        /// <param name="userName"></param>
        /// <param name="hostName"></param>
        /// <param name="connectionString"></param>
        /// <param name="filter"></param>
        public SqlLoggedSqlConnection(
            SqlLog.SqlLog sqlLog,
            int applicationId,
            string userName,
            string hostName,
            string connectionString,
            ISqlLoggedSqlCommandFilter filter)
        {
            Assert.IsNotNull(sqlLog);

            _sqlLog = sqlLog;
            _applicationId = applicationId;
            UserName = userName;
            HostName = hostName;
            Filter = filter;
            Connection = new SqlConnection(connectionString);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _sqlLog.DisposeConnection(Connection);
            }
        }

        IDbTransaction IDbConnection.BeginTransaction()
        {
            return Connection.BeginTransaction();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="il"></param>
        /// <returns></returns>
        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            return Connection.BeginTransaction(il);
        }

        void IDbConnection.ChangeDatabase(string databaseName)
        {
            Connection.ChangeDatabase(databaseName);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Close()
        {
            _sqlLog.CloseConnection(Connection);
        }

        IDbCommand IDbConnection.CreateCommand()
        {
            IDbCommand command = Connection.CreateCommand();
            return new SqlLoggedSqlCommand(this, command);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Open()
        {
            Exception exception = null;
            var startDate = LocalTime.Default.Now;
            var duration = Stopwatch.GetTimestamp();

            try
            {
                Connection.Open();
            }
            catch (Exception e)
            {
                exception = e;
                throw;
            }
            finally
            {
                duration = Stopwatch.GetTimestamp() - duration;
                _connectionNo = _sqlLog.ConnectionOpen(_applicationId, Connection, UserName, HostName, startDate, duration, exception);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ConnectionString
        {
            get => Connection.ConnectionString;

            set => Connection.ConnectionString = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public int ConnectionTimeout => Connection.ConnectionTimeout;

        /// <summary>
        /// 
        /// </summary>
        public string Database => Connection.Database;

        /// <summary>
        /// 
        /// </summary>
        public ConnectionState State => Connection.State;

        internal void CommandExeucte(
            IDbCommand command,
            DateTime startDate,
            long duration,
            Exception exception)
        {
            _sqlLog.CommandExecute(_applicationId, _connectionNo, command, startDate, duration, exception);
        }

        internal int ExecuteNonQuery(IDbCommand command)
        {
            int count;
            Exception exception = null;
            var startDate = LocalTime.Default.Now;
            var duration = Stopwatch.GetTimestamp();

            try
            {
                count = command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                exception = e;
                throw;
            }
            finally
            {
                duration = Stopwatch.GetTimestamp() - duration;
                var contains = exception != null || Filter == null || Filter.Contains(UserName, HostName, command);

                if (contains)
                {
                    _sqlLog.CommandExecute(_applicationId, _connectionNo, command, startDate, duration, exception);
                }
            }

            return count;
        }

        internal object ExecuteScalar(IDbCommand command)
        {
            object scalar = null;

            Exception exception = null;
            var startDate = LocalTime.Default.Now;
            var duration = Stopwatch.GetTimestamp();

            try
            {
                scalar = command.ExecuteScalar();
            }
            catch (Exception e)
            {
                exception = e;
                throw;
            }
            finally
            {
                duration = Stopwatch.GetTimestamp() - duration;
                var contains = exception != null || Filter == null || Filter.Contains(UserName, HostName, command);

                if (contains)
                {
                    _sqlLog.CommandExecute(_applicationId, _connectionNo, command, startDate, duration, exception);
                }
            }

            return scalar;
        }

        /// <summary>
        /// 
        /// </summary>
        public ISqlLoggedSqlCommandFilter Filter { get; }

        /// <summary>
        /// 
        /// </summary>
        public string UserName { get; }

        /// <summary>
        /// 
        /// </summary>
        public string HostName { get; }

        internal SqlConnection Connection { get; }
    }
}