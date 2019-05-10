using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Foundation.Assertions;

namespace DataCommander.Providers.Connection
{
    public abstract class ConnectionBase
    {
        public IDbConnection Connection { get; protected set; }
        public abstract Task OpenAsync(CancellationToken cancellationToken);

        public void Close()
        {
            if (Connection != null)
                Connection.Close();
        }

        public abstract IDbCommand CreateCommand();
        public abstract string ConnectionName { get; set; }
        public string ConnectionString => Connection.ConnectionString;
        public abstract string Caption { get; }
        public abstract string DataSource { get; }
        protected abstract void SetDatabase(string database);

        public string Database
        {
            get
            {
                var database = Connection?.Database;
                return database;
            }

            set => SetDatabase(value);
        }

        public abstract string ServerVersion { get; }

        public ConnectionState State
        {
            get
            {
                Assert.IsTrue(Connection != null);
                return Connection.State;
            }
        }

        public abstract int TransactionCount { get; }
        protected void InvokeInfoMessage(IReadOnlyCollection<InfoMessage> messages) => InfoMessage?.Invoke(messages);
        public event InfoMessageEventHandler InfoMessage;
        public event EventHandler<DatabaseChangedEventArgs> DatabaseChanged;
    }
}