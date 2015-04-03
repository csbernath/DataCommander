namespace DataCommander.Providers.SqlServerCe
{
    using System;
    using System.Data;
    using System.Data.SqlServerCe;

    internal sealed class Connection : ConnectionBase
    {
        private readonly SqlCeConnection sqlCeConnection;
        private string connectionName;

        public Connection( string connectionString )
        {
            this.sqlCeConnection = new SqlCeConnection( connectionString );
            this.Connection = this.sqlCeConnection;
        }

        public override void Open()
        {
            this.sqlCeConnection.Open();
        }

        public override IDbCommand CreateCommand()
        {
            return this.sqlCeConnection.CreateCommand();
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
                return this.sqlCeConnection.DataSource;
            }
        }

        public override string DataSource
        {
            get
            {
                return this.sqlCeConnection.DataSource;
            }
        }

        protected override void SetDatabase( string database )
        {
            throw new NotImplementedException();
        }

        public override string ServerVersion
        {
            get
            {
                return this.sqlCeConnection.ServerVersion;
            }
        }

        public override int TransactionCount
        {
            get
            {
                // TODO
                return 0;
            }
        }
    }
}