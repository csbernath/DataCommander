namespace DataCommander.Foundation.Data.SqlClient.SqlLog
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Text;
    using DataCommander.Foundation.Diagnostics;

    internal sealed class SqlLogCommand : ISqlLogItem
    {
        #region Private Fields

        private readonly int _applicationId;
        private readonly IDictionary<string, SqLoglCommandExecution> _commands;
        private readonly int _connectionNo;
        private readonly string _database;
        private readonly CommandType _commandType;
        private string _commandText;
        private string _parameters;
        private DateTime _startDate;
        private readonly long _duration;
        private readonly Exception _exception;

        #endregion

        public SqlLogCommand(
            int applicationId,
            IDictionary<string, SqLoglCommandExecution> commands,
            int connectionNo,
            IDbCommand command,
            DateTime startDate,
            long duration,
            Exception exception)
        {
            this._applicationId = applicationId;
            this._commands = commands;
            this._connectionNo = connectionNo;
            this._database = command.Connection.Database;
            this._commandType = command.CommandType;
            this._commandText = command.CommandText;
            var parameters = (SqlParameterCollection)command.Parameters;

            if (parameters.Count > 0)
            {
                this._parameters = parameters.ToLogString();
            }

            this._startDate = startDate;
            this._duration = duration;
            this._exception = exception;
        }

        string ISqlLogItem.CommandText
        {
            get
            {
                switch (this._commandType)
                {
                    case CommandType.Text:
                        var index = this._commandText.IndexOf(' ');

                        if (index >= 0)
                        {
                            var word = this._commandText.Substring(0, index);
                            word = word.ToLower();

                            switch (word)
                            {
                                case "exec":
                                case "execute":
                                    var startIndex = word.Length + 1;
                                    var endIndex = this._commandText.IndexOf(' ', startIndex);

                                    if (endIndex >= 0)
                                    {
                                        word = this._commandText.Substring(0, endIndex);
                                        this._parameters = this._commandText.Substring(endIndex + 1);
                                        this._commandText = word;
                                    }

                                    break;
                            }
                        }

                        break;

                    default:
                        break;
                }

                bool isNew;
                var command = this.GetCommandExecution(this._database, this._commandText, out isNew);
                var sb = new StringBuilder();

                if (isNew)
                {
                    sb.AppendFormat( "exec LogCommand {0},{1},{2},{3}\r\n", this._applicationId, command.CommandNo, this._database.ToTSqlVarChar(), this._commandText.ToTSqlVarChar() );
                }

                sb.AppendFormat(
                    CultureInfo.InvariantCulture,
                    "exec LogCommandExecute {0},{1},{2},{3},",
                    this._applicationId,
                    this._connectionNo,
                    command.CommandNo,
                    command.ExecutionNo);

                sb.Append( this._parameters.ToTSqlVarChar() );
                sb.Append(',');
                sb.Append( this._startDate.ToTSqlDateTime() );

                var microseconds = StopwatchTimeSpan.ToInt32(this._duration, 1000000);
                sb.AppendFormat(",{0}\r\n", microseconds);

                if (this._exception != null)
                {
                    var error = new SqlLogError(this._applicationId, this._connectionNo, command.CommandNo, command.ExecutionNo, this._exception);
                    sb.Append(error.CommandText);
                }

                return sb.ToString();
            }
        }

        private SqLoglCommandExecution GetCommandExecution(
            string database,
            string commandText,
            out bool isNew)
        {
            SqLoglCommandExecution command;
            isNew = false;
            var key = string.Format(CultureInfo.InvariantCulture, "{0}.{1}", database, commandText);

            lock (this._commands)
            {
                if (this._commands.TryGetValue( key, out command ))
                {
                    command.ExecutionNo++;
                }
                else
                {
                    var commandNo = this._commands.Count + 1;
                    command = new SqLoglCommandExecution( commandNo );
                    this._commands.Add( key, command );
                    isNew = true;
                }
            }

            return command;
        }
    }
}