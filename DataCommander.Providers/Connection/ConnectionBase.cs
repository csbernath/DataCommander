namespace DataCommander.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.Contracts;
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class ConnectionBase
    {
		private IDbConnection connection;

		public IDbConnection Connection
		{
            get
            {
                return this.connection;
            }

			protected set
			{
				this.connection = value;
			}
		}

        //public abstract void Open();

        public abstract Task OpenAsync(CancellationToken cancellationToken);

        public void Close()
        {
            if (this.connection != null)
            {
                this.connection.Close();
            }
        }

        public abstract IDbCommand CreateCommand();

        public abstract string ConnectionName
        {
            get;
            set;
        }

        public string ConnectionString
        {
            get
            {
                return this.connection.ConnectionString;
            }
        }

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

                if (this.connection != null)
                {
                    database = this.connection.Database;
                }
                else
                {
                    database = null;
                }

                return database;
            }

            set
            {
                this.SetDatabase(value);
            }
        }

        public abstract string ServerVersion
        {
            get;
        }

        public ConnectionState State
        {
            get
            {
                Contract.Assert(this.connection != null);
                return this.connection.State;
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