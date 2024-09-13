using System;
using System.Data;
using System.Data.Common;
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
    private readonly ConnectionStringAndCredential _connectionStringAndCredential;
    private SqlConnection _sqlConnection;
    private string _serverName;
    private short _serverProcessId;

    public Connection(ConnectionStringAndCredential connectionStringAndCredential)
    {
        _connectionStringAndCredential = connectionStringAndCredential;
        CreateConnection();
    }

    public override string ConnectionName { get; set; }

    public override string Caption
    {
        get
        {
            var sqlConnectionStringBuilder = new SqlConnectionStringBuilder(_sqlConnection.ConnectionString);
            var userName = sqlConnectionStringBuilder.IntegratedSecurity
                ? WindowsIdentity.GetCurrent().Name
                : sqlConnectionStringBuilder.UserID;
            var caption = $"{_sqlConnection.DataSource}.{_sqlConnection.Database} ({userName} ({_serverProcessId}))";
            return caption;
        }
    }

    public override string DataSource => _sqlConnection.DataSource;
    public override string ServerVersion => _sqlConnection.ServerVersion;

    public override string ConnectionInformation
    {
        get
        {
            var executor = _sqlConnection.CreateCommandExecutor();
            var commandText = "select @@version";
            var version = (string)executor.ExecuteScalar(new CreateCommandRequest(commandText));
            var serverVersion = _sqlConnection.ServerVersion;
            var contains = SqlServerVersionInfoRepository.TryGetByVersion(serverVersion, out var sqlServerVersionInfo);
            var description = contains ? sqlServerVersionInfo.Name : null;
            return @$"Server name: {_serverName}
{version}
{description}";
        }
    }

    public override async Task<int> GetTransactionCountAsync(CancellationToken cancellationToken)
    {
        var executor = _sqlConnection.CreateCommandAsyncExecutor();
        var scalar = await executor.ExecuteScalarAsync(new CreateCommandRequest("select @@trancount"), cancellationToken);
        var transactionCount = (int)scalar;
        return transactionCount;
    }

    private void CreateConnection()
    {
        var sqlConnectionStringBuilder = new SqlConnectionStringBuilder(_connectionStringAndCredential.ConnectionString)
        {
            ApplicationName = "Data Commander",
            Pooling = false
        };
        var credential = _connectionStringAndCredential.Credential;
        var sqlCredential = credential != null
            ? new SqlCredential(credential.UserId, credential.Password.SecureString)
            : null;
        _sqlConnection = new SqlConnection(sqlConnectionStringBuilder.ConnectionString, sqlCredential);
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
            _serverProcessId = (short)_sqlConnection.ServerProcessId;

            const string commandText = @"select @@servername
set arithabort on";

            var executor = DbCommandExecutorFactory.Create(_sqlConnection);
            var items = await executor.ExecuteReaderAsync(
                new ExecuteReaderRequest(commandText),
                1,
                dataRecord => new { ServerName = dataRecord.GetString(0) },
                cancellationToken);
            var item = items.First();
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
                var percentString = error.Message[..index];
                var percent = int.Parse(percentString);
                var remainingPercent = 100 - percent;
                var estimated = (long)Math.Round(100.0 / percent * elapsed);
                var estimatedRemaining = remainingPercent * elapsed / percent;
                var infoMessage = new InfoMessage(localTime, InfoMessageSeverity.Verbose, null,
                    $"Estimated time: {StopwatchTimeSpan.ToString(estimated, 0)} remaining time: {StopwatchTimeSpan.ToString(estimatedRemaining, 0)}, finishes at: {LocalTime.Default.Now.AddSeconds(estimatedRemaining * StopwatchConstants.SecondsPerTick)}");
                infoMessages.Add(infoMessage);
            }
        }

        InvokeInfoMessage(infoMessages);
    }

    public override DbCommand CreateCommand()
    {
        _createCommandTimestamp = Stopwatch.GetTimestamp();
        return _sqlConnection.CreateCommand();
    }
}