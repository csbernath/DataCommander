namespace DataCommander.Providers.Odp
{
    using System;
    using System.Data;
    using System.Text;
    using DataCommander.Foundation;
    using Oracle.ManagedDataAccess.Client;

    internal sealed class Connection : ConnectionBase
    {
		private readonly OracleConnectionStringBuilder oracleConnectionStringBuilder;
		private readonly OracleConnection oracleConnection;
		private string connectionName;

        public Connection(string connectionString)
        {
            this.oracleConnectionStringBuilder = new OracleConnectionStringBuilder(connectionString);            
            this.oracleConnection = new OracleConnection(connectionString);
            this.Connection = oracleConnection;
            oracleConnection.InfoMessage += new OracleInfoMessageEventHandler(OnInfoMessage);
        }

        public override void Open()
        {
            oracleConnection.Open();
            bool enlist = bool.Parse(oracleConnectionStringBuilder.Enlist);
        }

        public override string Caption
        {
            get
            {
                string caption = string.Format("{0}@{1}", oracleConnectionStringBuilder.UserID, oracleConnectionStringBuilder.DataSource);
                return caption;
            }
        }

        private void OnInfoMessage( object sender, OracleInfoMessageEventArgs e )
        {
            DateTime now = LocalTime.Default.Now;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine( e.Message );
            sb.Append( "Source: " );
            sb.Append( e.Source );

            InvokeInfoMessage( new InfoMessage[] { new InfoMessage( now, InfoMessageSeverity.Information, sb.ToString() ) } );
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

        public override string DataSource
        {
            get
            {
                return this.oracleConnection.DataSource;
            }
        }

        public override string ServerVersion
        {
            get
            {
                return this.oracleConnection.ServerVersion;
            }
        }

        public override IDbCommand CreateCommand()
        {
            OracleCommand command = this.oracleConnection.CreateCommand();
            command.InitialLONGFetchSize = 8 * 1024;
            command.FetchSize = 256 * 1024;
            return command;
        }

        protected override void SetDatabase(string database)
        {
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