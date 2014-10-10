namespace DataCommander.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.Contracts;

    public enum InfoMessageSeverity
    {
        Error,
        Information,
        Verbose
    }

    public class DatabaseChangedEventArgs : EventArgs
    {
        public string database;
    }

    public sealed class InfoMessage
    {
        private DateTime creationTime;
        private InfoMessageSeverity severity;
        private String message;

        public InfoMessage( 
            DateTime creationTime,
            InfoMessageSeverity severity,
            string message )
        {
            this.creationTime = creationTime;
            this.severity = severity;
            this.message = message;
        }

        public DateTime CreationTime
        {
            get
            {
                return this.creationTime;
            }
        }

        public InfoMessageSeverity Severity
        {
            get
            {
                return this.severity;
            }
        }

        public String Message
        {
            get
            {
                return this.message;
            }
        }
    }

    public delegate void InfoMessageEventHandler( IEnumerable<InfoMessage> messages );

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

        public abstract void Open();

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
                return connection.ConnectionString;
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

                if (connection != null)
                {
                    database = connection.Database;
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