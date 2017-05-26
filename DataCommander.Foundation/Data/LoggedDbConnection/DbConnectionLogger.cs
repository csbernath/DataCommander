using System.Data.Common;
using Foundation.Diagnostics;
using Foundation.Diagnostics.Log;
using Foundation.Linq;

namespace Foundation.Data.LoggedDbConnection
{
    internal sealed class DbConnectionLogger
    {
        #region Private Fields

        private static readonly ILog Log = LogFactory.Instance.GetTypeLog(typeof (DbConnectionLogger));
        private LoggedDbConnection _connection;
        private BeforeOpenDbConnectionEventArgs _beforeOpen;
        private BeforeExecuteCommandEventArgs _beforeExecuteReader;

        #endregion

        public DbConnectionLogger(LoggedDbConnection connection)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(connection != null);
#endif

            this._connection = connection;

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

            Log.Trace("Opening connection {0}...", csb.ConnectionString);

            this._beforeOpen = e;
        }

        private void ConnectionAfterOpen(object sender, AfterOpenDbConnectionEventArgs e)
        {
            var duration = e.Timestamp - this._beforeOpen.Timestamp;
            if (e.Exception != null)
            {
                Log.Write(LogLevel.Error, "Opening connection finished in {0} seconds. Exception:\r\n{1}", StopwatchTimeSpan.ToString(duration, 3), e.Exception.ToLogString());
            }
            else
            {
                Log.Trace("Opening connection finished in {0} seconds.", StopwatchTimeSpan.ToString(duration, 3));
            }

            this._beforeOpen = null;
        }

        private void ConnectionBeforeExecuteReader(object sender, BeforeExecuteCommandEventArgs e)
        {
            this._beforeExecuteReader = e;
        }

        private static string ToString(LoggedDbCommandInfo command, long duration)
        {
            return
                $"Executing command started in {StopwatchTimeSpan.ToString(duration, 3)} seconds.\r\ncommandId: {command.CommandId},connectionState: {command.ConnectionState},database: {command.Database},executionType: {command.ExecutionType},commandType: {command.CommandType},commandTimeout: {command.CommandTimeout}\r\ncommandText: {command.CommandText}\r\nparameters:\r\n{command.Parameters}";
        }

        private void ConnectionAfterExecuteReader(object sender, AfterExecuteCommandEventArgs e)
        {
            var duration = e.Timestamp - this._beforeExecuteReader.Timestamp;
            if (e.Exception != null)
            {
                Log.Write(LogLevel.Error, "{0}\r\nException:\r\n{1}", ToString(e.Command, duration), e.Exception.ToLogString());
                this._beforeExecuteReader = null;
            }
            else
            {
                Log.Trace("{0}", ToString(e.Command, duration));
            }
        }

        private void ConnectionAfterRead(object sender, AfterReadEventArgs e)
        {
            var duration = e.Timestamp - this._beforeExecuteReader.Timestamp;
            Log.Trace("{0} row(s) read in {1} seconds.", e.RowCount, StopwatchTimeSpan.ToString(duration, 3));
        }
    }
}