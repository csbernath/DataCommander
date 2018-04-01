using System;
using System.Data;
using Foundation.Assertions;

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
            Assert.IsNotNull(connection);

            _connection = connection;
        }

#region Public Events

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<BeforeOpenDbConnectionEventArgs> BeforeOpen
        {
            add => _beforeOpen += value;

            remove => _beforeOpen -= value;
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<AfterOpenDbConnectionEventArgs> AfterOpen
        {
            add => _afterOpen += value;

            remove => _afterOpen -= value;
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<BeforeExecuteCommandEventArgs> BeforeExecuteReader
        {
            add => _beforeExecuteCommand += value;

            remove => _beforeExecuteCommand -= value;
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<AfterExecuteCommandEventArgs> AfterExecuteReader
        {
            add => _afterExecuteCommand += value;

            remove => _afterExecuteCommand -= value;
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<AfterReadEventArgs> AfterRead
        {
            add => _afterRead += value;

            remove => _afterRead -= value;
        }

#endregion

#region IDbConnection Members

        IDbTransaction IDbConnection.BeginTransaction(IsolationLevel il)
        {
            return _connection.BeginTransaction(il);
        }

        IDbTransaction IDbConnection.BeginTransaction()
        {
            return _connection.BeginTransaction();
        }

        void IDbConnection.ChangeDatabase(string databaseName)
        {
            _connection.ChangeDatabase(databaseName);
        }

        void IDbConnection.Close()
        {
            _connection.Close();
        }

        string IDbConnection.ConnectionString
        {
            get => _connection.ConnectionString;

            set => _connection.ConnectionString = value;
        }

        int IDbConnection.ConnectionTimeout => _connection.ConnectionTimeout;

        IDbCommand IDbConnection.CreateCommand()
        {
            var command = _connection.CreateCommand();
            return new LoggedDbCommand(command, _beforeExecuteCommand, _afterExecuteCommand, _afterRead);
        }

        string IDbConnection.Database => _connection.Database;

        void IDbConnection.Open()
        {
            if (_beforeOpen != null)
            {
                var eventArgs = new BeforeOpenDbConnectionEventArgs(_connection.ConnectionString);
                _beforeOpen(this, eventArgs);
            }

            if (_afterOpen != null)
            {
                Exception exception = null;
                try
                {
                    _connection.Open();
                }
                catch (Exception e)
                {
                    exception = e;
                    throw;
                }
                finally
                {
                    var eventArgs = new AfterOpenDbConnectionEventArgs(exception);
                    _afterOpen(this, eventArgs);
                }
            }
            else
            {
                _connection.Open();
            }
        }

        ConnectionState IDbConnection.State => _connection.State;

#endregion

#region IDisposable Members

        void IDisposable.Dispose()
        {
            _connection.Dispose();
        }

#endregion
    }
}