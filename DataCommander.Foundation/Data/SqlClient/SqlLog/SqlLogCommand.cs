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

        private readonly Int32 applicationId;
        private IDictionary<String, SqLoglCommandExecution> commands;
        private Int32 connectionNo;
        private String database;
        private CommandType commandType;
        private String commandText;
        private String parameters;
        private DateTime startDate;
        private Int64 duration;
        private Exception exception;

        #endregion

        public SqlLogCommand(
            Int32 applicationId,
            IDictionary<String, SqLoglCommandExecution> commands,
            Int32 connectionNo,
            IDbCommand command,
            DateTime startDate,
            Int64 duration,
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

        String ISqlLogItem.CommandText
        {
            get
            {
                switch (this.commandType)
                {
                    case CommandType.Text:
                        Int32 index = this.commandText.IndexOf(' ');

                        if (index >= 0)
                        {
                            String word = this.commandText.Substring(0, index);
                            word = word.ToLower();

                            switch (word)
                            {
                                case "exec":
                                case "execute":
                                    Int32 startIndex = word.Length + 1;
                                    Int32 endIndex = this.commandText.IndexOf(' ', startIndex);

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

                Boolean isNew;
                SqLoglCommandExecution command = this.GetCommandExecution(this.database, this.commandText, out isNew);
                StringBuilder sb = new StringBuilder();

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

                Int32 microseconds = StopwatchTimeSpan.ToInt32(this.duration, 1000000);
                sb.AppendFormat(",{0}\r\n", microseconds);

                if (this.exception != null)
                {
                    SqlLogError error = new SqlLogError(this.applicationId, this.connectionNo, command.CommandNo, command.ExecutionNo, this.exception);
                    sb.Append(error.CommandText);
                }

                return sb.ToString();
            }
        }

        private SqLoglCommandExecution GetCommandExecution(
            String database,
            String commandText,
            out Boolean isNew)
        {
            SqLoglCommandExecution command;
            isNew = false;
            String key = String.Format(CultureInfo.InvariantCulture, "{0}.{1}", database, commandText);

            lock (this.commands)
            {
                if (this.commands.TryGetValue( key, out command ))
                {
                    command.ExecutionNo++;
                }
                else
                {
                    Int32 commandNo = this.commands.Count + 1;
                    command = new SqLoglCommandExecution( commandNo );
                    this.commands.Add( key, command );
                    isNew = true;
                }
            }

            return command;
        }
    }
}