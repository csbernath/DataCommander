namespace DataCommander.Providers.MySql
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using DataCommander.Providers;
    using global::MySql.Data.MySqlClient;

    internal sealed class Connection : ConnectionBase
    {
        private string connectionString;
        private MySqlConnection mySqlConnection;
        private string connectionName;

        public Connection(string connectionString)
        {
            this.connectionString = connectionString;
            this.mySqlConnection = new MySqlConnection(connectionString);
            this.Connection = this.mySqlConnection;
        }

        public override void Open()
        {
            this.mySqlConnection.Open();
        }

        public override System.Data.IDbCommand CreateCommand()
        {
            return this.mySqlConnection.CreateCommand();
        }

        public override string ConnectionName
        {
            get
            {
                return this.connectionName;
            }

            set
            {
                this.connectionName = value;
            }
        }

        public override string Caption
        {
            get
            {
                return this.connectionName;
            }
        }

        public override string DataSource
        {
            get
            {
                // TODO
                var csb = new MySqlConnectionStringBuilder(this.connectionString);
                return csb.Database;
            }
        }

        protected override void SetDatabase(string database)
        {
            throw new NotImplementedException();
        }

        public override string ServerVersion
        {
            get
            {
                return this.mySqlConnection.ServerVersion;
            }
        }

        public override int TransactionCount
        {
            get
            {
                return 0; // TODO
            }
        }
    }
}
