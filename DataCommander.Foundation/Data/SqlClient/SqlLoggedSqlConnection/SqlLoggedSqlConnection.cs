namespace DataCommander.Foundation.Data.SqlClient
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics;

    /// <summary>
    /// Logged SqlConnection class.
    /// </summary>
    public sealed class SqlLoggedSqlConnection : IDbConnection
    {
        private readonly int applicationId;
        private int connectionNo;
        private readonly SqlLog sqlLog;

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
            SqlLog sqlLog,
            int applicationId,
            string userName,
            string hostName,
            string connectionString,
            ISqlLoggedSqlCommandFilter filter)
        {
#if CONTRACTS_FULL
            Contract.Requires(sqlLog != null);
#endif

            this.sqlLog = sqlLog;
            this.applicationId = applicationId;
            this.UserName = userName;
            this.HostName = hostName;
            this.Filter = filter;
            this.Connection = new SqlConnection(connectionString);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
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
                this.sqlLog.DisposeConnection(this.Connection);
            }
        }

        IDbTransaction IDbConnection.BeginTransaction()
        {
            return this.Connection.BeginTransaction();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="il"></param>
        /// <returns></returns>
        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            return this.Connection.BeginTransaction(il);
        }

        void IDbConnection.ChangeDatabase(string databaseName)
        {
            this.Connection.ChangeDatabase(databaseName);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Close()
        {
            this.sqlLog.CloseConnection(this.Connection);
        }

        IDbCommand IDbConnection.CreateCommand()
        {
            IDbCommand command = this.Connection.CreateCommand();
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
                this.Connection.Open();
            }
            catch (Exception e)
            {
                exception = e;
                throw;
            }
            finally
            {
                duration = Stopwatch.GetTimestamp() - duration;
                this.connectionNo = this.sqlLog.ConnectionOpen(this.applicationId, this.Connection, this.UserName, this.HostName, startDate, duration, exception);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return this.Connection.ConnectionString;
            }

            set
            {
                this.Connection.ConnectionString = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int ConnectionTimeout => this.Connection.ConnectionTimeout;

        /// <summary>
        /// 
        /// </summary>
        public string Database => this.Connection.Database;

        /// <summary>
        /// 
        /// </summary>
        public ConnectionState State => this.Connection.State;

        internal void CommandExeucte(
            IDbCommand command,
            DateTime startDate,
            long duration,
            Exception exception)
        {
            this.sqlLog.CommandExecute(this.applicationId, this.connectionNo, command, startDate, duration, exception);
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
                var contains = exception != null || this.Filter == null || this.Filter.Contains(this.UserName, this.HostName, command);

                if (contains)
                {
                    this.sqlLog.CommandExecute(this.applicationId, this.connectionNo, command, startDate, duration, exception);
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
                var contains = exception != null || this.Filter == null || this.Filter.Contains(this.UserName, this.HostName, command);

                if (contains)
                {
                    this.sqlLog.CommandExecute(this.applicationId, this.connectionNo, command, startDate, duration, exception);
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