namespace DataCommander.Providers.OracleClient
{
    using System;
    using System.Data;
    using System.Data.OracleClient;
    using System.Text;
    using DataCommander.Foundation;

    internal sealed class Connection : ConnectionBase
    {
        private string connectionName;

        public Connection( string connectionString )
        {
            oracleConnection = new OracleConnection( connectionString );
            this.Connection = oracleConnection;
            oracleConnection.InfoMessage += new OracleInfoMessageEventHandler( OnInfoMessage );
        }

        public override void Open()
        {
            oracleConnection.Open();
        }

        public override string Caption
        {
            get
            {
                //string caption = string.Format("{0}@{1}",connectionString.UserId,oracleConnection.DataSource);
                //return caption;
                return null;
            }
        }

        void OnInfoMessage( object sender, OracleInfoMessageEventArgs e )
        {
            DateTime now = LocalTime.Default.Now;

            StringBuilder sb = new StringBuilder();
            sb.Append( e.Message );
            sb.Append( Environment.NewLine );
            sb.Append( "Code: " );
            sb.Append( e.Code );
            sb.Append( Environment.NewLine );
            sb.Append( "Source: " );
            sb.Append( e.Source );

            this.InvokeInfoMessage( new InfoMessage[] { new InfoMessage( now, InfoMessageSeverity.Information, sb.ToString() ) } );
        }

        public override string DataSource
        {
            get
            {
                return oracleConnection.DataSource;
            }
        }

        public override string ServerVersion
        {
            get
            {
                return oracleConnection.ServerVersion;
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
        
        public override IDbCommand CreateCommand()
        {
            return oracleConnection.CreateCommand();
        }

        protected override void SetDatabase( string database )
        {
        }

        readonly OracleConnection oracleConnection;

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
    }
}