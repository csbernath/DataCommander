namespace DataCommander.Providers.SqlServer2005
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Security.Principal;
    using DataCommander.Foundation;

    internal sealed class Connection : ConnectionBase
    {
        #region Private Fields

        private string connectionName;
        private readonly SqlConnectionStringBuilder sqlConnectionStringBuilder;
        private SqlConnection sqlConnection;
        private string serverName;
        private short spid;

        #endregion

        public Connection(string connectionString)
        {
            this.sqlConnectionStringBuilder = new SqlConnectionStringBuilder(connectionString)
            {
                ApplicationName = "Data Commander",
                Pooling = false
            };

            this.CreateConnection();
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

        private void CreateConnection()
        {
            this.sqlConnection = new SqlConnection(this.sqlConnectionStringBuilder.ConnectionString);
            this.Connection = this.sqlConnection;
            this.sqlConnection.FireInfoMessageEventOnUserErrors = true;
            this.sqlConnection.InfoMessage += this.OnInfoMessage;
            this.sqlConnection.StateChange += this.OnStateChange;
        }

        private void OnStateChange(object sender, StateChangeEventArgs e)
        {
            DateTime now = LocalTime.Default.Now;
            string text = string.Format("Connection.State changed. OriginalState: {0}, CurrentState: {1}", e.OriginalState, e.CurrentState);
            this.InvokeInfoMessage
                (
                    new InfoMessage[]
                    {
                        new InfoMessage(now, InfoMessageSeverity.Information, text)
                    }
                );
        }

        public override void Open()
        {
            this.sqlConnection.Open();
            var table = this.sqlConnection.ExecuteDataTable(@"select @@servername,@@spid
set arithabort on");
            var row = table.Rows[0];
            this.serverName = (string)row[0];
            this.spid = (short)row[1];
        }

        public override string Caption
        {
            get
            {
                string userName = null;

                if (this.sqlConnectionStringBuilder.IntegratedSecurity)
                {
                    userName = WindowsIdentity.GetCurrent().Name;
                }
                else
                {
                    userName = this.sqlConnectionStringBuilder.UserID;
                }

                string caption = string.Format("{0}.{1} ({2} ({3}))", this.sqlConnection.DataSource, this.sqlConnection.Database,
                    userName, this.spid);

                return caption;
            }
        }

        private void OnInfoMessage(object sender, SqlInfoMessageEventArgs e)
        {
            var infoMessages = SqlServerProvider.ToInfoMessages(e.Errors);
            this.InvokeInfoMessage(infoMessages);
        }

        public override string DataSource
        {
            get
            {
                return this.sqlConnection.DataSource;
            }
        }

        public override string ServerVersion
        {
            get
            {
                string commandText = "select @@version";
                object scalar = this.sqlConnection.ExecuteScalar(commandText);
                string version = Foundation.Data.Database.GetValueOrDefault<string>(scalar);

                /* select
          serverproperty('Collation') as Collation,
          serverproperty('Edition') as Edition,
          serverproperty('Engine Edition') as [Engine Edition],
          serverproperty('InstanceName') as InstanceName,
          serverproperty('IsClustered') as IsClustered,
          serverproperty('IsFullTextInstalled') as IsFullTextInstalled,
          serverproperty('IsIntegratedSecurityOnly') as IsIntegratedSecurityOnly,
          serverproperty('IsSingleUser') as IsSingleUser,
          serverproperty('IsSyncWithBackup') as IsSyncWithBackup,
          serverproperty('LicenseType') as LicenseType,
          serverproperty('MachineName') as MachineName,
          serverproperty('NumLicenses') as NumLicenses,
          serverproperty('ProcessID') as ProcessID,
          serverproperty('ProductVersion') as ProductVersion,
          serverproperty('ProductLevel') as ProductLevel,
          serverproperty('ServerName') as ServerName
          */
                string serverVersion = this.sqlConnection.ServerVersion;
                string description;

                switch (serverVersion)
                {
                    case "08.00.0194":
                        description = "SQL Server 2000 RTM";
                        break;

                    case "08.00.0760":
                        description = "SQL Server 2000 SP3";
                        break;

                    case "08.00.0878":
                        description = "SQL Server 2000 SP3a";
                        break;

                    case "08.00.2039":
                        description = "SQL Server 2000 SP4";
                        break;

                    case "08.00.2040":
                        description = "SQL Server 2000 (after SP4)";
                        break;

                    case "08.00.2187":
                        description = "SQL Server 2000 post SP4 hotfix build (build 2187)";
                        break;

                    case "09.00.1399":
                        description = "SQL Server 2005";
                        break;

                    case "09.00.2047":
                        description = "SQL Server 2005 (before SP1)";
                        break;

                    case "09.00.2153":
                        description = "SQL Server 2005 SP1";
                        break;

                    case "09.00.3042":
                        description = "SQL Server 2005 SP2";
                        break;

                    case "09.00.3073":
                        description = "SQL Server 2005 SP2 + 954606 MS08-052: Description of the security update for GDI+ for SQL Server 2005 Service Pack 2 GDR: September 9, 2008";
                        break;

                    case "09.00.3080":
                        description = "SQL Server 2005 SP2 + 970895 MS09-062: Description of the security update for GDI+ for SQL Server 2005 Service Pack 2 GDR: October 13, 2009";
                        break;

                    case "09.00.3186":
                        description = "SQL Server 2005 SP2 + cumulative update (August 20, 2007)";
                        break;

                    case "09.00.4035":
                        description = "SQL Server 2005 Service Pack 3";
                        break;

                    case "09.00.4053":
                        description = "SQL Server 2005 Service Pack 3 GDR: October 13, 2009";
                        break;

                    case "09.00.5000":
                        description = "SQL Server 2005 Service Pack 4 (SP4): December 17, 2010";
                        break;

                    case "09.00.5057":
                        description = "Security update for SQL Server 2005 Service Pack 4 GDR: June 14, 2011";
                        break;

                    case "10.00.1600":
                        description = "SQL Server 2008 (RTM)";
                        break;

                    default:
                        description = null;
                        break;
                }

                return string.Format("{0}\r\n{1}\r\n@@version: {2}\r\n@@servername: {3}", serverVersion, description, version, this.serverName);
            }
        }

        public override int TransactionCount
        {
            get
            {
                object scalar = this.sqlConnection.ExecuteScalar("select @@trancount");
                int transactionCount = (int)scalar;
                return transactionCount;
            }
        }

        public override IDbCommand CreateCommand()
        {
            return this.sqlConnection.CreateCommand();
        }

        protected override void SetDatabase(string database)
        {
            this.sqlConnectionStringBuilder.InitialCatalog = database;
            this.sqlConnection.Dispose();
            this.sqlConnection = null;
            this.CreateConnection();
        }
    }
}