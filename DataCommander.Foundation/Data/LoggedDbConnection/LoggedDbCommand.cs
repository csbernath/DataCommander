namespace DataCommander.Foundation.Data
{
    using System;
    using System.Data;
    using System.Diagnostics.Contracts;
    using System.Threading;

    internal sealed class LoggedDbCommand : IDbCommand
    {
        #region Private Fields

        private static int commandIdCounter;
        private readonly int commandId;
        private readonly IDbCommand command;
        private readonly EventHandler<BeforeExecuteCommandEventArgs> beforeExecuteCommand;
        private readonly EventHandler<AfterExecuteCommandEventArgs> afterExecuteCommand;
        private readonly EventHandler<AfterReadEventArgs> afterRead;

        #endregion

        public LoggedDbCommand(
            IDbCommand command,
            EventHandler<BeforeExecuteCommandEventArgs> beforeExecuteCommand,
            EventHandler<AfterExecuteCommandEventArgs> afterExecuteCommand,
            EventHandler<AfterReadEventArgs> afterRead)
        {
            Contract.Requires<ArgumentNullException>(command != null);
            Contract.Requires<ArgumentNullException>(beforeExecuteCommand != null);
            Contract.Requires<ArgumentNullException>(afterExecuteCommand != null);
            Contract.Requires<ArgumentNullException>(afterRead != null);

            Contract.Ensures(this.command != null);

            this.commandId = Interlocked.Increment(ref commandIdCounter);
            this.command = command;
            this.beforeExecuteCommand = beforeExecuteCommand;
            this.afterExecuteCommand = afterExecuteCommand;
            this.afterRead = afterRead;
        }

        #region IDbCommand Members

        void IDbCommand.Cancel()
        {
            this.command.Cancel();
        }

        string IDbCommand.CommandText
        {
            get
            {
                return this.command.CommandText;
            }

            set
            {
                this.command.CommandText = value;
            }
        }

        int IDbCommand.CommandTimeout
        {
            get
            {
                return this.command.CommandTimeout;
            }

            set
            {
                this.command.CommandTimeout = value;
            }
        }

        CommandType IDbCommand.CommandType
        {
            get
            {
                return this.command.CommandType;
            }

            set
            {
                this.command.CommandType = value;
            }
        }

        IDbConnection IDbCommand.Connection
        {
            get
            {
                return this.command.Connection;
            }

            set
            {
                this.command.Connection = value;
            }
        }

        IDbDataParameter IDbCommand.CreateParameter()
        {
            return this.command.CreateParameter();
        }

        int IDbCommand.ExecuteNonQuery()
        {
            var commandInfo = new Lazy<LoggedDbCommandInfo>(() => this.CreateLoggedDbCommandInfo(LoggedDbCommandExecutionType.NonQuery));

            if (this.beforeExecuteCommand != null)
            {
                var eventArgs = new BeforeExecuteCommandEventArgs(commandInfo.Value);
                this.beforeExecuteCommand(this, eventArgs);
            }

            int rowCount;

            if (this.afterExecuteCommand != null)
            {
                Exception exception = null;
                try
                {
                    rowCount = this.command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    exception = e;
                    throw;
                }
                finally
                {
                    var eventArgs = new AfterExecuteCommandEventArgs(commandInfo.Value, exception);
                    this.afterExecuteCommand(this, eventArgs);
                }
            }
            else
            {
                rowCount = this.command.ExecuteNonQuery();
            }

            return rowCount;
        }

        IDataReader IDbCommand.ExecuteReader(CommandBehavior behavior)
        {
            var commandInfo = new Lazy<LoggedDbCommandInfo>(() => this.CreateLoggedDbCommandInfo(LoggedDbCommandExecutionType.Reader));

            if (this.beforeExecuteCommand != null)
            {
                var eventArgs = new BeforeExecuteCommandEventArgs(commandInfo.Value);
                this.beforeExecuteCommand(this, eventArgs);
            }

            IDataReader dataReader;

            if (this.afterExecuteCommand != null)
            {
                Exception exception = null;
                try
                {
                    dataReader = this.command.ExecuteReader();
                }
                catch (Exception e)
                {
                    exception = e;
                    throw;
                }
                finally
                {
                    var eventArgs = new AfterExecuteCommandEventArgs(commandInfo.Value, exception);
                    this.afterExecuteCommand(this, eventArgs);
                }
            }
            else
            {
                dataReader = this.command.ExecuteReader();
            }

            return new LoggedDataReader(dataReader, this.afterRead);
        }

        IDataReader IDbCommand.ExecuteReader()
        {
            var dbCommand = (IDbCommand)this;
            return dbCommand.ExecuteReader(CommandBehavior.Default);
        }

        object IDbCommand.ExecuteScalar()
        {
            var commandInfo = new Lazy<LoggedDbCommandInfo>(() => this.CreateLoggedDbCommandInfo(LoggedDbCommandExecutionType.Scalar));
            if (this.beforeExecuteCommand != null)
            {
                var eventArgs = new BeforeExecuteCommandEventArgs(commandInfo.Value);
                this.beforeExecuteCommand(this, eventArgs);
            }

            object scalar;

            if (this.afterExecuteCommand != null)
            {
                Exception exception = null;
                try
                {
                    scalar = this.command.ExecuteScalar();
                }
                catch (Exception e)
                {
                    exception = e;
                    throw;
                }
                finally
                {
                    var args = new AfterExecuteCommandEventArgs(commandInfo.Value, exception);
                    this.afterExecuteCommand(this, args);
                }
            }
            else
            {
                scalar = this.command.ExecuteScalar();
            }

            return scalar;
        }

        IDataParameterCollection IDbCommand.Parameters
        {
            get
            {
                return this.command.Parameters;
            }
        }

        void IDbCommand.Prepare()
        {
            this.command.Prepare();
        }

        IDbTransaction IDbCommand.Transaction
        {
            get
            {
                return this.command.Transaction;
            }

            set
            {
                this.command.Transaction = value;
            }
        }

        UpdateRowSource IDbCommand.UpdatedRowSource
        {
            get
            {
                return this.command.UpdatedRowSource;
            }

            set
            {
                this.command.UpdatedRowSource = value;
            }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            this.command.Dispose();
        }

        #endregion

        #region Private Methods

        private LoggedDbCommandInfo CreateLoggedDbCommandInfo(LoggedDbCommandExecutionType executionType)
        {
            var connection = this.command.Connection;

            return new LoggedDbCommandInfo(
                this.commandId,
                connection.State,
                connection.Database,
                executionType,
                this.command.CommandType,
                this.command.CommandTimeout,
                this.command.CommandText,
                this.command.Parameters.ToLogString());
        }

        #endregion
    }
}