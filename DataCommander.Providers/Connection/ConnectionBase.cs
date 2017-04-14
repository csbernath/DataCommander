namespace DataCommander.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class ConnectionBase
    {
        public IDbConnection Connection { get; protected set; }

        //public abstract void Open();

        public abstract Task OpenAsync(CancellationToken cancellationToken);

        public void Close()
        {
            if (this.Connection != null)
            {
                this.Connection.Close();
            }
        }

        public abstract IDbCommand CreateCommand();

        public abstract string ConnectionName
        {
            get;
            set;
        }

        public string ConnectionString => this.Connection.ConnectionString;

        public abstract string Caption
        {
            get;
        }

        public abstract string DataSource
        {
            get;
        }

        protected abstract void SetDatabase(string database);

        public string Database
        {
            get
            {
                string database;

                if (this.Connection != null)
                {
                    database = this.Connection.Database;
                }
                else
                {
                    database = null;
                }

                return database;
            }

            set => this.SetDatabase(value);
        }

        public abstract string ServerVersion
        {
            get;
        }

        public ConnectionState State
        {
            get
            {
#if CONTRACTS_FULL
                Contract.Assert(this.Connection != null);
#endif
                return this.Connection.State;
            }
        }

        public abstract int TransactionCount
        {
            get;
        }

        protected void InvokeInfoMessage(IEnumerable<InfoMessage> messages)
        {
            if (this.InfoMessage != null)
            {
                this.InfoMessage( messages );
            }
        }

        protected void InvokeDatabaseChanged(string database)
        {
            if (this.DatabaseChanged != null)
            {
                var args = new DatabaseChangedEventArgs
                {
                    database = database
                };
                this.DatabaseChanged( this, args );
            }
        }

        public event InfoMessageEventHandler InfoMessage;

        public event EventHandler<DatabaseChangedEventArgs> DatabaseChanged;
    }
}