namespace DataCommander.Foundation.Data.SqlClient
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

        private readonly int applicationId;
        private readonly IDictionary<string, SqLoglCommandExecution> commands;
        private readonly int connectionNo;
        private readonly string database;
        private readonly CommandType commandType;
        private string commandText;
        private string parameters;
        private DateTime startDate;
        private readonly long duration;
        private readonly Exception exception;

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
            this.applicationId = applicationId;
            this.commands = commands;
            this.connectionNo = connectionNo;
            this.database = command.Connection.Database;
            this.commandType = command.CommandType;
            this.commandText = command.CommandText;
            var parameters = (SqlParameterCollection)command.Parameters;

            if (parameters.Count > 0)
            {
                this.parameters = parameters.ToLogString();
            }

            this.startDate = startDate;
            this.duration = duration;
            this.exception = exception;
        }

        string ISqlLogItem.CommandText
        {
            get
            {
                switch (this.commandType)
                {
                    case CommandType.Text:
                        var index = this.commandText.IndexOf(' ');

                        if (index >= 0)
                        {
                            var word = this.commandText.Substring(0, index);
                            word = word.ToLower();

                            switch (word)
                            {
                                case "exec":
                                case "execute":
                                    var startIndex = word.Length + 1;
                                    var endIndex = this.commandText.IndexOf(' ', startIndex);

                                    if (endIndex >= 0)
                                    {
                                        word = this.commandText.Substring(0, endIndex);
                                        this.parameters = this.commandText.Substring(endIndex + 1);
                                        this.commandText = word;
                                    }

                                    break;
                            }
                        }

                        break;

                    default:
                        break;
                }

                bool isNew;
                var command = this.GetCommandExecution(this.database, this.commandText, out isNew);
                var sb = new StringBuilder();

                if (isNew)
                {
                    sb.AppendFormat( "exec LogCommand {0},{1},{2},{3}\r\n", this.applicationId, command.CommandNo, this.database.ToTSqlVarChar(), this.commandText.ToTSqlVarChar() );
                }

                sb.AppendFormat(
                    CultureInfo.InvariantCulture,
                    "exec LogCommandExecute {0},{1},{2},{3},",
                    this.applicationId,
                    this.connectionNo,
                    command.CommandNo,
                    command.ExecutionNo);

                sb.Append( this.parameters.ToTSqlVarChar() );
                sb.Append(',');
                sb.Append( this.startDate.ToTSqlDateTime() );

                var microseconds = StopwatchTimeSpan.ToInt32(this.duration, 1000000);
                sb.AppendFormat(",{0}\r\n", microseconds);

                if (this.exception != null)
                {
                    var error = new SqlLogError(this.applicationId, this.connectionNo, command.CommandNo, command.ExecutionNo, this.exception);
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

            lock (this.commands)
            {
                if (this.commands.TryGetValue( key, out command ))
                {
                    command.ExecutionNo++;
                }
                else
                {
                    var commandNo = this.commands.Count + 1;
                    command = new SqLoglCommandExecution( commandNo );
                    this.commands.Add( key, command );
                    isNew = true;
                }
            }

            return command;
        }
    }
}