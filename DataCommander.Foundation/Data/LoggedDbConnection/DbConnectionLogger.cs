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

        private static readonly ILog log = LogFactory.Instance.GetTypeLog(typeof (DbConnectionLogger));
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
            var duration = e.Timestamp - this.beforeOpen.Timestamp;
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
            return
                $"Executing command started in {StopwatchTimeSpan.ToString(duration, 3)} seconds.\r\ncommandId: {command.CommandId},connectionState: {command.ConnectionState},database: {command.Database},executionType: {command.ExecutionType},commandType: {command.CommandType},commandTimeout: {command.CommandTimeout}\r\ncommandText: {command.CommandText}\r\nparameters:\r\n{command.Parameters}";
        }

        private void ConnectionAfterExecuteReader(object sender, AfterExecuteCommandEventArgs e)
        {
            var duration = e.Timestamp - this.beforeExecuteReader.Timestamp;
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
            var duration = e.Timestamp - this.beforeExecuteReader.Timestamp;
            log.Trace("{0} row(s) read in {1} seconds.", e.RowCount, StopwatchTimeSpan.ToString(duration, 3));
        }
    }
}