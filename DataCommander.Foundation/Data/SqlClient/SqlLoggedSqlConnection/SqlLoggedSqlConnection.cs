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
        private readonly Int32 applicationId;
        private Int32 connectionNo;
        private readonly String userName;
        private readonly String hostName;
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
            Int32 applicationId,
            String userName,
            String hostName,
            String connectionString,
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
        private void Dispose(Boolean disposing)
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

        void IDbConnection.ChangeDatabase(String databaseName)
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
            DateTime startDate = OptimizedDateTime.Now;
            Int64 duration = Stopwatch.GetTimestamp();

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
        public String ConnectionString
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
        public Int32 ConnectionTimeout
        {
            get
            {
                return this.connection.ConnectionTimeout;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String Database
        {
            get
            {
                return this.connection.Database;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ConnectionState State
        {
            get
            {
                return this.connection.State;
            }
        }

        internal void CommandExeucte(
            IDbCommand command,
            DateTime startDate,
            Int64 duration,
            Exception exception)
        {
            this.sqlLog.CommandExecute(this.applicationId, this.connectionNo, command, startDate, duration, exception);
        }

        internal Int32 ExecuteNonQuery(IDbCommand command)
        {
            Int32 count;
            Exception exception = null;
            DateTime startDate = OptimizedDateTime.Now;
            Int64 duration = Stopwatch.GetTimestamp();

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
                Boolean contains = exception != null || this.filter == null || this.filter.Contains(this.userName, this.hostName, command);

                if (contains)
                {
                    this.sqlLog.CommandExecute(this.applicationId, this.connectionNo, command, startDate, duration, exception);
                }
            }

            return count;
        }

        internal Object ExecuteScalar(IDbCommand command)
        {
            Object scalar = null;

            Exception exception = null;
            DateTime startDate = OptimizedDateTime.Now;
            Int64 duration = Stopwatch.GetTimestamp();

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
                Boolean contains = exception != null || this.filter == null || this.filter.Contains(this.userName, this.hostName, command);

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
        public ISqlLoggedSqlCommandFilter Filter
        {
            get
            {
                return this.filter;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String UserName
        {
            get
            {
                return this.userName;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String HostName
        {
            get
            {
                return this.hostName;
            }
        }

        internal SqlConnection Connection
        {
            get
            {
                return this.connection;
            }
        }
    }
}