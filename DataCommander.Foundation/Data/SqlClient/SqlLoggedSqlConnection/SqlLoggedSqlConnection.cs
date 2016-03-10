namespace DataCommander.Foundation.Data.SqlClient
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Logged SqlConnection class.
    /// </summary>
    public sealed class SqlLoggedSqlConnection : IDbConnection
    {
        private readonly SqlConnection connection;
        private readonly int applicationId;
        private int connectionNo;
        private readonly string userName;
        private readonly string hostName;
        private readonly SqlLog sqlLog;
        private readonly ISqlLoggedSqlCommandFilter filter;

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
            Contract.Requires(sqlLog != null);

            this.sqlLog = sqlLog;
            this.applicationId = applicationId;
            this.userName = userName;
            this.hostName = hostName;
            this.filter = filter;
            this.connection = new SqlConnection(connectionString);
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
                this.sqlLog.DisposeConnection(this.connection);
            }
        }

        IDbTransaction IDbConnection.BeginTransaction()
        {
            return this.connection.BeginTransaction();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="il"></param>
        /// <returns></returns>
        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            return this.connection.BeginTransaction(il);
        }

        void IDbConnection.ChangeDatabase(string databaseName)
        {
            this.connection.ChangeDatabase(databaseName);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Close()
        {
            this.sqlLog.CloseConnection(this.connection);
        }

        IDbCommand IDbConnection.CreateCommand()
        {
            IDbCommand command = this.connection.CreateCommand();
            return new SqlLoggedSqlCommand(this, command);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Open()
        {
            Exception exception = null;
            DateTime startDate = LocalTime.Default.Now;
            long duration = Stopwatch.GetTimestamp();

            try
            {
                this.connection.Open();
            }
            catch (Exception e)
            {
                exception = e;
                throw;
            }
            finally
            {
                duration = Stopwatch.GetTimestamp() - duration;
                this.connectionNo = this.sqlLog.ConnectionOpen(this.applicationId, this.connection, this.userName, this.hostName, startDate, duration, exception);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return this.connection.ConnectionString;
            }

            set
            {
                this.connection.ConnectionString = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int ConnectionTimeout => this.connection.ConnectionTimeout;

        /// <summary>
        /// 
        /// </summary>
        public string Database => this.connection.Database;

        /// <summary>
        /// 
        /// </summary>
        public ConnectionState State => this.connection.State;

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
            DateTime startDate = LocalTime.Default.Now;
            long duration = Stopwatch.GetTimestamp();

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
                bool contains = exception != null || this.filter == null || this.filter.Contains(this.userName, this.hostName, command);

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
            DateTime startDate = LocalTime.Default.Now;
            long duration = Stopwatch.GetTimestamp();

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
                bool contains = exception != null || this.filter == null || this.filter.Contains(this.userName, this.hostName, command);

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
        public ISqlLoggedSqlCommandFilter Filter => this.filter;

        /// <summary>
        /// 
        /// </summary>
        public string UserName => this.userName;

        /// <summary>
        /// 
        /// </summary>
        public string HostName => this.hostName;

        internal SqlConnection Connection => this.connection;
    }
}