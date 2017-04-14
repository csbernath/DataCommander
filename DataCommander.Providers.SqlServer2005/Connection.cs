namespace DataCommander.Providers.SqlServer2005
{
    using System.Data;
    using System.Data.SqlClient;
    using System.Security.Principal;
    using System.Threading;
    using System.Threading.Tasks;
    using DataCommander.Foundation;
    using Foundation.Data;

    internal sealed class Connection : ConnectionBase
    {
        #region Private Fields

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

        public override string ConnectionName { get; set; }

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
            var now = LocalTime.Default.Now;
            var text = $"Connection.State changed. OriginalState: {e.OriginalState}, CurrentState: {e.CurrentState}";
            this.InvokeInfoMessage
                (
                    new InfoMessage[]
                    {
                        new InfoMessage(now, InfoMessageSeverity.Information, text)
                    }
                );
        }

        public override async Task OpenAsync(CancellationToken cancellationToken)
        {
            await this.sqlConnection.OpenAsync(cancellationToken);

            if (!cancellationToken.IsCancellationRequested)
            {
                const string commandText = @"select @@servername,@@spid
set arithabort on";

                var transactionScope = new DbTransactionScope(this.sqlConnection, null);

                using (var dataReader = transactionScope.ExecuteReader(new CommandDefinition {CommandText = commandText}, CommandBehavior.Default))
                {
                    dataReader.Read(dataRecord =>
                    {
                        this.serverName = dataRecord.GetString(0);
                        this.spid = dataRecord.GetInt16(1);
                        return false;
                    });
                }
            }
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

                var caption = $"{this.sqlConnection.DataSource}.{this.sqlConnection.Database} ({userName} ({this.spid}))";

                return caption;
            }
        }

        private void OnInfoMessage(object sender, SqlInfoMessageEventArgs e)
        {
            var infoMessages = SqlServerProvider.ToInfoMessages(e.Errors);
            this.InvokeInfoMessage(infoMessages);
        }

        public override string DataSource => this.sqlConnection.DataSource;

        public override string ServerVersion
        {
            get
            {
                var transactionScope = new DbTransactionScope(this.sqlConnection, null);
                var commandText = "select @@version";
                var version = transactionScope.ExecuteScalar<string>(new CommandDefinition {CommandText = commandText});

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
                var serverVersion = this.sqlConnection.ServerVersion;
                string description;

                switch (serverVersion)
                {
                        #region SQL Server 2000

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

                        #endregion

                        #region SQL Server 2005

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
                        description =
                            "SQL Server 2005 SP2 + 954606 MS08-052: Description of the security update for GDI+ for SQL Server 2005 Service Pack 2 GDR: September 9, 2008";
                        break;

                    case "09.00.3080":
                        description =
                            "SQL Server 2005 SP2 + 970895 MS09-062: Description of the security update for GDI+ for SQL Server 2005 Service Pack 2 GDR: October 13, 2009";
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

                        #endregion

                        #region SQL Server 2008

                    case "10.00.1600":
                        description = "SQL Server 2008 (RTM)";
                        break;

                    case "10.50.1600":
                        description = "SQL Server 2008 R2 RTM: April 21, 2010";
                        break;

                    case "10.50.6000":
                        description = "SQL Server 2008 R2 Service Pack 3 (SP3): September 26, 2014";
                        break;

                        #endregion

                        #region SQL Server 2012

                    case "11.00.2100":
                        description = "SQL Server 2012 RTM: March 6, 2012";
                        break;

                    case "11.00.5058":
                        description = "SQL Server 2012 Service Pack 2 (SP2): June 10, 2014";
                        break;

                        #endregion

                        #region SQL Server 2014

                    case "12.00.2430":
                        description = "2999197 Cumulative update package 4 (CU4) for SQL Server 2014: October 21, 2014";
                        break;

                        #endregion

                    default:
                        description = null;
                        break;
                }

                return $"{serverVersion}\r\n{description}\r\n@@version: {version}\r\n@@servername: {this.serverName}";
            }
        }

        public override int TransactionCount
        {
            get
            {
                var transactionScope = new DbTransactionScope(this.sqlConnection, null);
                var scalar = transactionScope.ExecuteScalar(new CommandDefinition {CommandText = "select @@trancount"});
                var transactionCount = (int)scalar;
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