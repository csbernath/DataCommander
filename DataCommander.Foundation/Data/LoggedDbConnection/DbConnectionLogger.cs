namespace DataCommander.Foundation.Data
{
    using System;
    using System.Data.Common;
    using System.Diagnostics.Contracts;
    using DataCommander.Foundation.Diagnostics;
    using DataCommander.Foundation.Linq;

    internal sealed class DbConnectionLogger
    {
        #region Private Fields

        private static readonly ILog log = LogFactory.Instance.GetCurrentTypeLog();
        private LoggedDbConnection connection;
        private BeforeOpenDbConnectionEventArgs beforeOpen;
        private BeforeExecuteCommandEventArgs beforeExecuteReader;

        #endregion

        public DbConnectionLogger(LoggedDbConnection connection)
        {
            Contract.Requires<ArgumentNullException>(connection != null);

            this.connection = connection;

            connection.BeforeOpen += this.ConnectionBeforeOpen;
            connection.AfterOpen += this.ConnectionAfterOpen;
            connection.BeforeExecuteReader += this.ConnectionBeforeExecuteReader;
            connection.AfterExecuteReader += this.ConnectionAfterExecuteReader;
            connection.AfterRead += this.ConnectionAfterRead;
        }

        private void ConnectionBeforeOpen(object sender, BeforeOpenDbConnectionEventArgs e)
        {
            var csb = new DbConnectionStringBuilder {ConnectionString = e.ConnectionString};

            if (csb.ContainsKey("Password"))
            {
                csb["Password"] = "<not logged here>";
            }

            log.Trace("Opening connection {0}...", csb.ConnectionString);

            this.beforeOpen = e;
        }

        private void ConnectionAfterOpen(object sender, AfterOpenDbConnectionEventArgs e)
        {
            long duration = e.Timestamp - this.beforeOpen.Timestamp;
            if (e.Exception != null)
            {
                log.Write(LogLevel.Error, "Opening connection finished in {0} seconds. Exception:\r\n{1}", StopwatchTimeSpan.ToString(duration, 3), e.Exception.ToLogString());
            }
            else
            {
                log.Trace("Opening connection finished in {0} seconds.", StopwatchTimeSpan.ToString(duration, 3));
            }

            this.beforeOpen = null;
        }

        private void ConnectionBeforeExecuteReader(object sender, BeforeExecuteCommandEventArgs e)
        {
            this.beforeExecuteReader = e;
        }

        private static string ToString(LoggedDbCommandInfo command, long duration)
        {
            return string.Format(
                "Executing command started in {0} seconds.\r\ncommandId: {1},connectionState: {2},database: {3},executionType: {4},commandType: {5},commandTimeout: {6}\r\ncommandText: {7}\r\nparameters:\r\n{8}",
                StopwatchTimeSpan.ToString(duration, 3),
                command.CommandId,
                command.ConnectionState,
                command.Database,
                command.ExecutionType,
                command.CommandType,
                command.CommandTimeout,
                command.CommandText,
                command.Parameters);
        }

        private void ConnectionAfterExecuteReader(object sender, AfterExecuteCommandEventArgs e)
        {
            long duration = e.Timestamp - this.beforeExecuteReader.Timestamp;
            if (e.Exception != null)
            {
                log.Write(LogLevel.Error, "{0}\r\nException:\r\n{1}", ToString(e.Command, duration), e.Exception.ToLogString());
                this.beforeExecuteReader = null;
            }
            else
            {
                log.Trace("{0}", ToString(e.Command, duration));
            }
        }

        private void ConnectionAfterRead(object sender, AfterReadEventArgs e)
        {
            long duration = e.Timestamp - this.beforeExecuteReader.Timestamp;
            log.Trace("{0} row(s) read in {1} seconds.", e.RowCount, StopwatchTimeSpan.ToString(duration, 3));
        }
    }
}