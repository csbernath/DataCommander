namespace SqlUtil.Providers.Odbc
{
    using System.Data;
    using System.Data.Odbc;

    sealed class Connection : ConnectionBase
    {
        public Connection(string connectionString)
        {
            odbcConnection = new OdbcConnection(connectionString);
            base.connection = odbcConnection;

            odbcConnection.InfoMessage += new OdbcInfoMessageEventHandler(OnInfoMessage);
        }

        public override void Open()
        {
            odbcConnection.Open();
        }

        public override string Caption
        {
            get
            {
                return "odbc";
            }
        }

        void OnInfoMessage(object sender,OdbcInfoMessageEventArgs e)
        {
        }

        public override string DataSource
        {
            get
            {
                return odbcConnection.DataSource;
            }
        }

        public override string ServerVersion
        {
            get
            {
                return odbcConnection.ServerVersion;
            }
        }

        public override IDbCommand CreateCommand()
        {
            return odbcConnection.CreateCommand();
        }

        protected override void SetDatabase(string database)
        {
        }

        OdbcConnection odbcConnection;
    }
}