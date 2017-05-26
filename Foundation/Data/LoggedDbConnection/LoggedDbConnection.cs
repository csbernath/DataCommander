using System;
using System.Data;

namespace Foundation.Data.LoggedDbConnection
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class LoggedDbConnection : IDbConnection
    {
        private readonly IDbConnection _connection;
        private EventHandler<BeforeOpenDbConnectionEventArgs> _beforeOpen;
        private EventHandler<AfterOpenDbConnectionEventArgs> _afterOpen;
        private EventHandler<BeforeExecuteCommandEventArgs> _beforeExecuteCommand;
        private EventHandler<AfterExecuteCommandEventArgs> _afterExecuteCommand;
        private EventHandler<AfterReadEventArgs> _afterRead;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        public LoggedDbConnection(IDbConnection connection)
        {
#if CONTRACTS_FULL
            Contract.Requires(connection != null);
#endif

            this._connection = connection;
        }

#region Public Events

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<BeforeOpenDbConnectionEventArgs> BeforeOpen
        {
            add => this._beforeOpen += value;

            remove => this._beforeOpen -= value;
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<AfterOpenDbConnectionEventArgs> AfterOpen
        {
            add => this._afterOpen += value;

            remove => this._afterOpen -= value;
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<BeforeExecuteCommandEventArgs> BeforeExecuteReader
        {
            add => this._beforeExecuteCommand += value;

            remove => this._beforeExecuteCommand -= value;
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<AfterExecuteCommandEventArgs> AfterExecuteReader
        {
            add => this._afterExecuteCommand += value;

            remove => this._afterExecuteCommand -= value;
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<AfterReadEventArgs> AfterRead
        {
            add => this._afterRead += value;

            remove => this._afterRead -= value;
        }

#endregion

#region IDbConnection Members

        IDbTransaction IDbConnection.BeginTransaction(IsolationLevel il)
        {
            return this._connection.BeginTransaction(il);
        }

        IDbTransaction IDbConnection.BeginTransaction()
        {
            return this._connection.BeginTransaction();
        }

        void IDbConnection.ChangeDatabase(string databaseName)
        {
            this._connection.ChangeDatabase(databaseName);
        }

        void IDbConnection.Close()
        {
            this._connection.Close();
        }

        string IDbConnection.ConnectionString
        {
            get => this._connection.ConnectionString;

            set => this._connection.ConnectionString = value;
        }

        int IDbConnection.ConnectionTimeout => this._connection.ConnectionTimeout;

        IDbCommand IDbConnection.CreateCommand()
        {
            var command = this._connection.CreateCommand();
            return new LoggedDbCommand(command, this._beforeExecuteCommand, this._afterExecuteCommand, this._afterRead);
        }

        string IDbConnection.Database => this._connection.Database;

        void IDbConnection.Open()
        {
            if (this._beforeOpen != null)
            {
                var eventArgs = new BeforeOpenDbConnectionEventArgs(this._connection.ConnectionString);
                this._beforeOpen(this, eventArgs);
            }

            if (this._afterOpen != null)
            {
                Exception exception = null;
                try
                {
                    this._connection.Open();
                }
                catch (Exception e)
                {
                    exception = e;
                    throw;
                }
                finally
                {
                    var eventArgs = new AfterOpenDbConnectionEventArgs(exception);
                    this._afterOpen(this, eventArgs);
                }
            }
            else
            {
                this._connection.Open();
            }
        }

        ConnectionState IDbConnection.State => this._connection.State;

#endregion

#region IDisposable Members

        void IDisposable.Dispose()
        {
            this._connection.Dispose();
        }

#endregion
    }
}