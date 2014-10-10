namespace DataCommander.Foundation.Data.SqlClient
{
    using System;
    using System.Data;
    using System.Diagnostics.Contracts;

    internal sealed class SqlLoggedSqlCommand : IDbCommand
    {
        private readonly SqlLoggedSqlConnection connection;
        private readonly IDbCommand command;

        public SqlLoggedSqlCommand(
            SqlLoggedSqlConnection connection,
            IDbCommand command)
        {
            Contract.Requires(connection != null);
            Contract.Requires(command != null);

            this.connection = connection;
            this.command = command;
        }

        public String CommandText
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

        public Int32 CommandTimeout
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

        public CommandType CommandType
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

        public IDbConnection Connection
        {
            get
            {
                return this.connection;
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public IDataParameterCollection Parameters
        {
            get
            {
                return this.command.Parameters;
            }
        }

        public IDbTransaction Transaction
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

        public UpdateRowSource UpdatedRowSource
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

        public void Dispose()
        {
            this.command.Dispose();
        }

        public void Cancel()
        {
            this.command.Cancel();
        }

        public IDbDataParameter CreateParameter()
        {
            return this.command.CreateParameter();
        }

        public Int32 ExecuteNonQuery()
        {
            return this.connection.ExecuteNonQuery(this.command);
        }

        public IDataReader ExecuteReader()
        {
            SqlLoggedSqlDataReader loggedSqlDataReader = new SqlLoggedSqlDataReader(this.connection, this.command);
            return loggedSqlDataReader.Execute();
        }

        public IDataReader ExecuteReader(CommandBehavior behavior)
        {
            SqlLoggedSqlDataReader loggedSqlDataReader = new SqlLoggedSqlDataReader(this.connection, this.command);
            return loggedSqlDataReader.Execute(behavior);
        }

        public Object ExecuteScalar()
        {
            return this.connection.ExecuteScalar(this.command);
        }

        public void Prepare()
        {
            this.command.Prepare();
        }
    }
}