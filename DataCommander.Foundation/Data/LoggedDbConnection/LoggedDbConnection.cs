namespace DataCommander.Foundation.Data.LoggedDbConnection
{
    using System;
    using System.Data;

    /// <summary>
    /// 
    /// </summary>
    public sealed class LoggedDbConnection : IDbConnection
    {
        private readonly IDbConnection connection;
        private EventHandler<BeforeOpenDbConnectionEventArgs> beforeOpen;
        private EventHandler<AfterOpenDbConnectionEventArgs> afterOpen;
        private EventHandler<BeforeExecuteCommandEventArgs> beforeExecuteCommand;
        private EventHandler<AfterExecuteCommandEventArgs> afterExecuteCommand;
        private EventHandler<AfterReadEventArgs> afterRead;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        public LoggedDbConnection(IDbConnection connection)
        {
#if CONTRACTS_FULL
            Contract.Requires(connection != null);
#endif

            this.connection = connection;
        }

#region Public Events

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<BeforeOpenDbConnectionEventArgs> BeforeOpen
        {
            add => this.beforeOpen += value;

            remove => this.beforeOpen -= value;
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<AfterOpenDbConnectionEventArgs> AfterOpen
        {
            add => this.afterOpen += value;

            remove => this.afterOpen -= value;
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<BeforeExecuteCommandEventArgs> BeforeExecuteReader
        {
            add => this.beforeExecuteCommand += value;

            remove => this.beforeExecuteCommand -= value;
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<AfterExecuteCommandEventArgs> AfterExecuteReader
        {
            add => this.afterExecuteCommand += value;

            remove => this.afterExecuteCommand -= value;
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<AfterReadEventArgs> AfterRead
        {
            add => this.afterRead += value;

            remove => this.afterRead -= value;
        }

#endregion

#region IDbConnection Members

        IDbTransaction IDbConnection.BeginTransaction(IsolationLevel il)
        {
            return this.connection.BeginTransaction(il);
        }

        IDbTransaction IDbConnection.BeginTransaction()
        {
            return this.connection.BeginTransaction();
        }

        void IDbConnection.ChangeDatabase(string databaseName)
        {
            this.connection.ChangeDatabase(databaseName);
        }

        void IDbConnection.Close()
        {
            this.connection.Close();
        }

        string IDbConnection.ConnectionString
        {
            get => this.connection.ConnectionString;

            set => this.connection.ConnectionString = value;
        }

        int IDbConnection.ConnectionTimeout => this.connection.ConnectionTimeout;

        IDbCommand IDbConnection.CreateCommand()
        {
            var command = this.connection.CreateCommand();
            return new LoggedDbCommand(command, this.beforeExecuteCommand, this.afterExecuteCommand, this.afterRead);
        }

        string IDbConnection.Database => this.connection.Database;

        void IDbConnection.Open()
        {
            if (this.beforeOpen != null)
            {
                var eventArgs = new BeforeOpenDbConnectionEventArgs(this.connection.ConnectionString);
                this.beforeOpen(this, eventArgs);
            }

            if (this.afterOpen != null)
            {
                Exception exception = null;
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
                    var eventArgs = new AfterOpenDbConnectionEventArgs(exception);
                    this.afterOpen(this, eventArgs);
                }
            }
            else
            {
                this.connection.Open();
            }
        }

        ConnectionState IDbConnection.State => this.connection.State;

#endregion

#region IDisposable Members

        void IDisposable.Dispose()
        {
            this.connection.Dispose();
        }

#endregion
    }
}