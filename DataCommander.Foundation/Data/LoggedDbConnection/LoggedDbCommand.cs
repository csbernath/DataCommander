namespace DataCommander.Foundation.Data.LoggedDbConnection
{
    using System;
    using System.Data;
    using System.Threading;

    internal sealed class LoggedDbCommand : IDbCommand
    {
        #region Private Fields

        private static int _commandIdCounter;
        private readonly int _commandId;
        private readonly IDbCommand _command;
        private readonly EventHandler<BeforeExecuteCommandEventArgs> _beforeExecuteCommand;
        private readonly EventHandler<AfterExecuteCommandEventArgs> _afterExecuteCommand;
        private readonly EventHandler<AfterReadEventArgs> _afterRead;

        #endregion

        public LoggedDbCommand(
            IDbCommand command,
            EventHandler<BeforeExecuteCommandEventArgs> beforeExecuteCommand,
            EventHandler<AfterExecuteCommandEventArgs> afterExecuteCommand,
            EventHandler<AfterReadEventArgs> afterRead)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(command != null);
            Contract.Requires<ArgumentNullException>(beforeExecuteCommand != null);
            Contract.Requires<ArgumentNullException>(afterExecuteCommand != null);
            Contract.Requires<ArgumentNullException>(afterRead != null);

            Contract.Ensures(this.command != null);
#endif

            this._commandId = Interlocked.Increment(ref _commandIdCounter);
            this._command = command;
            this._beforeExecuteCommand = beforeExecuteCommand;
            this._afterExecuteCommand = afterExecuteCommand;
            this._afterRead = afterRead;
        }

#region IDbCommand Members

        void IDbCommand.Cancel()
        {
            this._command.Cancel();
        }

        string IDbCommand.CommandText
        {
            get => this._command.CommandText;

            set => this._command.CommandText = value;
        }

        int IDbCommand.CommandTimeout
        {
            get => this._command.CommandTimeout;

            set => this._command.CommandTimeout = value;
        }

        CommandType IDbCommand.CommandType
        {
            get => this._command.CommandType;

            set => this._command.CommandType = value;
        }

        IDbConnection IDbCommand.Connection
        {
            get => this._command.Connection;

            set => this._command.Connection = value;
        }

        IDbDataParameter IDbCommand.CreateParameter()
        {
            return this._command.CreateParameter();
        }

        int IDbCommand.ExecuteNonQuery()
        {
            var commandInfo = new Lazy<LoggedDbCommandInfo>(() => this.CreateLoggedDbCommandInfo(LoggedDbCommandExecutionType.NonQuery));

            if (this._beforeExecuteCommand != null)
            {
                var eventArgs = new BeforeExecuteCommandEventArgs(commandInfo.Value);
                this._beforeExecuteCommand(this, eventArgs);
            }

            int rowCount;

            if (this._afterExecuteCommand != null)
            {
                Exception exception = null;
                try
                {
                    rowCount = this._command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    exception = e;
                    throw;
                }
                finally
                {
                    var eventArgs = new AfterExecuteCommandEventArgs(commandInfo.Value, exception);
                    this._afterExecuteCommand(this, eventArgs);
                }
            }
            else
            {
                rowCount = this._command.ExecuteNonQuery();
            }

            return rowCount;
        }

        IDataReader IDbCommand.ExecuteReader(CommandBehavior behavior)
        {
            var commandInfo = new Lazy<LoggedDbCommandInfo>(() => this.CreateLoggedDbCommandInfo(LoggedDbCommandExecutionType.Reader));

            if (this._beforeExecuteCommand != null)
            {
                var eventArgs = new BeforeExecuteCommandEventArgs(commandInfo.Value);
                this._beforeExecuteCommand(this, eventArgs);
            }

            IDataReader dataReader;

            if (this._afterExecuteCommand != null)
            {
                Exception exception = null;
                try
                {
                    dataReader = this._command.ExecuteReader();
                }
                catch (Exception e)
                {
                    exception = e;
                    throw;
                }
                finally
                {
                    var eventArgs = new AfterExecuteCommandEventArgs(commandInfo.Value, exception);
                    this._afterExecuteCommand(this, eventArgs);
                }
            }
            else
            {
                dataReader = this._command.ExecuteReader();
            }

            return new LoggedDataReader(dataReader, this._afterRead);
        }

        IDataReader IDbCommand.ExecuteReader()
        {
            var dbCommand = (IDbCommand)this;
            return dbCommand.ExecuteReader(CommandBehavior.Default);
        }

        object IDbCommand.ExecuteScalar()
        {
            var commandInfo = new Lazy<LoggedDbCommandInfo>(() => this.CreateLoggedDbCommandInfo(LoggedDbCommandExecutionType.Scalar));
            if (this._beforeExecuteCommand != null)
            {
                var eventArgs = new BeforeExecuteCommandEventArgs(commandInfo.Value);
                this._beforeExecuteCommand(this, eventArgs);
            }

            object scalar;

            if (this._afterExecuteCommand != null)
            {
                Exception exception = null;
                try
                {
                    scalar = this._command.ExecuteScalar();
                }
                catch (Exception e)
                {
                    exception = e;
                    throw;
                }
                finally
                {
                    var args = new AfterExecuteCommandEventArgs(commandInfo.Value, exception);
                    this._afterExecuteCommand(this, args);
                }
            }
            else
            {
                scalar = this._command.ExecuteScalar();
            }

            return scalar;
        }

        IDataParameterCollection IDbCommand.Parameters => this._command.Parameters;

        void IDbCommand.Prepare()
        {
            this._command.Prepare();
        }

        IDbTransaction IDbCommand.Transaction
        {
            get => this._command.Transaction;

            set => this._command.Transaction = value;
        }

        UpdateRowSource IDbCommand.UpdatedRowSource
        {
            get => this._command.UpdatedRowSource;

            set => this._command.UpdatedRowSource = value;
        }

#endregion

#region IDisposable Members

        void IDisposable.Dispose()
        {
            this._command.Dispose();
        }

#endregion

#region Private Methods

        private LoggedDbCommandInfo CreateLoggedDbCommandInfo(LoggedDbCommandExecutionType executionType)
        {
            var connection = this._command.Connection;

            return new LoggedDbCommandInfo(
                this._commandId,
                connection.State,
                connection.Database,
                executionType,
                this._command.CommandType,
                this._command.CommandTimeout,
                this._command.CommandText,
                this._command.Parameters.ToLogString());
        }

#endregion
    }
}