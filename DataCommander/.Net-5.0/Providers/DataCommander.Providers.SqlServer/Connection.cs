using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api.Connection;
using Foundation.Core;
using Foundation.Core.ClockAggregate;
using Foundation.Data;
using Foundation.Data.SqlClient;
using Microsoft.Data.SqlClient;

namespace DataCommander.Providers.SqlServer;

internal sealed class Connection : ConnectionBase
{
    #region Private Fields

    private readonly SqlConnectionStringBuilder _sqlConnectionStringBuilder;
    private SqlConnection _sqlConnection;
    private string _serverName;
    private short _spid;

    #endregion

    public Connection(string connectionString)
    {
        _sqlConnectionStringBuilder = new SqlConnectionStringBuilder(connectionString)
        {
            ApplicationName = "Data Commander",
            Pooling = false
        };

        CreateConnection();
    }

    public override string ConnectionName { get; set; }

    public override string Caption
    {
        get
        {
            var userName = _sqlConnectionStringBuilder.IntegratedSecurity
                ? WindowsIdentity.GetCurrent().Name
                : _sqlConnectionStringBuilder.UserID;
            var caption = $"{_sqlConnection.DataSource}.{_sqlConnection.Database} ({userName} ({_spid}))";
            return caption;
        }
    }

    public override string DataSource => _sqlConnection.DataSource;

    public override string ServerVersion
    {
        get
        {
            var executor = _sqlConnection.CreateCommandExecutor();
            var commandText = "select @@version";
            var version = (string) executor.ExecuteScalar(new CreateCommandRequest(commandText));

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
            var serverVersion = _sqlConnection.ServerVersion;
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

                #region SQL Server 2016

                case "13.00.4001":
                    description = "Microsoft SQL Server 2016 Service Pack 1 (SP1)";
                    break;

                case "13.00.5026":
                    description = "Microsoft SQL Server 2016 Service Pack 2 (SP2)";
                    break;

                #endregion

                #region SQL Server 2017

                case "14.00.1000":
                    description = "Microsoft SQL Server 2017 (RTM)";
                    break;

                case "14.00.3045":
                    description = "Microsoft SQL Server 2017 (RTM-CU12) (KB4464082)";
                    break;

                case "14.00.3048":
                    description = "Microsoft SQL Server 2017 (RTM-CU13) (KB4466404)";
                    break;

                case "14.00.3162":
                    description = "Microsoft SQL Server 2017 (RTM-CU15) (KB4498951)";
                    break;

                #endregion

                #region SQL Server 2019

                case "15.00.2070":
                    description = "4517790 Servicing Update (GDR1) for SQL Server 2019 RTM";
                    break;

                case "15.00.2080":
                    description = "Microsoft SQL Server 2019 (RTM-GDR) (KB4583458)";
                    break;

                #endregion

                default:
                    description = null;
                    break;
            }

            return $"{serverVersion}\r\n{description}\r\n@@version: {version}\r\n@@servername: {_serverName}";
        }
    }

    public override int TransactionCount
    {
        get
        {
            var executor = DbCommandExecutorFactory.Create(_sqlConnection);
            var scalar = executor.ExecuteScalar(new CreateCommandRequest("select @@trancount"));
            var transactionCount = (int) scalar;
            return transactionCount;
        }
    }

    private void CreateConnection()
    {
        _sqlConnection = new SqlConnection(_sqlConnectionStringBuilder.ConnectionString);
        Connection = _sqlConnection;
        _sqlConnection.FireInfoMessageEventOnUserErrors = true;
        _sqlConnection.InfoMessage += OnInfoMessage;
        _sqlConnection.StateChange += OnStateChange;
    }

    private void OnStateChange(object sender, StateChangeEventArgs e)
    {
        var text = $"Connection.State changed. OriginalState: {e.OriginalState}, CurrentState: {e.CurrentState}";
        InvokeInfoMessage(new[]
        {
            InfoMessageFactory.Create(InfoMessageSeverity.Information, null, text)
        });
    }

    public override async Task OpenAsync(CancellationToken cancellationToken)
    {
        await _sqlConnection.OpenAsync(cancellationToken);

        if (!cancellationToken.IsCancellationRequested)
        {
            _spid = (short) _sqlConnection.ServerProcessId;

            const string commandText = @"select @@servername
set arithabort on";

            var executor = DbCommandExecutorFactory.Create(_sqlConnection);
            var item = executor.ExecuteReader(new ExecuteReaderRequest(commandText), 1, dataRecord => new
            {
                ServerName = dataRecord.GetString(0)
            }).First();
            _serverName = item.ServerName;
        }
    }

    private long _createCommandTimestamp;

    private void OnInfoMessage(object sender, SqlInfoMessageEventArgs e)
    {
        var clock = ClockAggregateRepository.Singleton.Get();
        var localTime = clock.GetLocalTimeFromCurrentEnvironmentTickCount();
        var infoMessages = SqlServerProvider.ToInfoMessages(e.Errors, localTime);

        if (e.Errors.Count > 0)
        {
            var error = e.Errors[0];
            if (error.Number == SqlErrorNumber.PercentProcessed)
            {
                var elapsed = Stopwatch.GetTimestamp() - _createCommandTimestamp;
                var index = error.Message.IndexOf(' ');
                var percentString = error.Message.Substring(0, index);
                var percent = int.Parse(percentString);
                var remainingPercent = 100 - percent;
                var estimated = (long) Math.Round(100.0 / percent * elapsed);
                var estimatedRemaining = remainingPercent * elapsed / percent;
                var infoMessage = new InfoMessage(localTime, InfoMessageSeverity.Verbose, null,
                    $"Estimated time: {StopwatchTimeSpan.ToString(estimated, 0)} remaining time: {StopwatchTimeSpan.ToString(estimatedRemaining, 0)}");
                infoMessages.Add(infoMessage);
            }
        }

        InvokeInfoMessage(infoMessages);
    }

    public override IDbCommand CreateCommand()
    {
        _createCommandTimestamp = Stopwatch.GetTimestamp();
        return _sqlConnection.CreateCommand();
    }

    protected override void SetDatabase(string database)
    {
        _sqlConnectionStringBuilder.InitialCatalog = database;
        _sqlConnection.Dispose();
        _sqlConnection = null;
        CreateConnection();
    }
}