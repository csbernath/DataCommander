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
            SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder(_sqlConnection.ConnectionString);
            string userName = sqlConnectionStringBuilder.IntegratedSecurity
                ? WindowsIdentity.GetCurrent().Name
                : sqlConnectionStringBuilder.UserID;
            string caption = $"{_sqlConnection.DataSource}.{_sqlConnection.Database} ({userName} ({_serverProcessId}))";
            return caption;
        }
    }

    public override string DataSource => _sqlConnection.DataSource;
    public override string ServerVersion => _sqlConnection.ServerVersion;

    public override string ConnectionInformation
    {
        get
        {
            IDbCommandExecutor executor = _sqlConnection.CreateCommandExecutor();
            string commandText = "select @@version";
            string version = (string)executor.ExecuteScalar(new CreateCommandRequest(commandText));
            string serverVersion = _sqlConnection.ServerVersion;
            bool contains = SqlServerVersionInfoRepository.TryGetByVersion(serverVersion, out SqlServerVersionInfo? sqlServerVersionInfo);
            string? description = contains ? sqlServerVersionInfo.Name : null;
            return @$"Server name: {_serverName}
{version}
{description}";
        }
    }

    public override async Task<int> GetTransactionCountAsync(CancellationToken cancellationToken)
    {
        IDbCommandAsyncExecutor executor = _sqlConnection.CreateCommandAsyncExecutor();
        object scalar = await executor.ExecuteScalarAsync(new CreateCommandRequest("select @@trancount"), cancellationToken);
        int transactionCount = (int)scalar;
        return transactionCount;
    }

    private void CreateConnection()
    {
        SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder(_connectionStringAndCredential.ConnectionString)
        {
            ApplicationName = "Data Commander",
            Pooling = false
        };
        Credential? credential = _connectionStringAndCredential.Credential;
        SqlCredential? sqlCredential = credential != null
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
        string text = $"Connection.State changed. OriginalState: {e.OriginalState}, CurrentState: {e.CurrentState}";
        InvokeInfoMessage(
        [
            InfoMessageFactory.Create(InfoMessageSeverity.Information, null, text)
        ]);
    }

    public override async Task OpenAsync(CancellationToken cancellationToken)
    {
        await _sqlConnection.OpenAsync(cancellationToken);

        if (!cancellationToken.IsCancellationRequested)
        {
            _serverProcessId = (short)_sqlConnection.ServerProcessId;

            const string commandText = @"select @@servername
set arithabort on";

            IDbCommandAsyncExecutor executor = DbCommandExecutorFactory.Create(_sqlConnection);
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
        ClockAggregateRoot clock = ClockAggregateRepository.Singleton.Get();
        DateTime localTime = clock.GetLocalTimeFromCurrentEnvironmentTickCount();
        System.Collections.Generic.List<InfoMessage> infoMessages = SqlServerProvider.ToInfoMessages(e.Errors, localTime);

        if (e.Errors.Count > 0)
        {
            SqlError error = e.Errors[0];
            if (error.Number == SqlErrorNumber.PercentProcessed)
            {
                long elapsed = Stopwatch.GetTimestamp() - _createCommandTimestamp;
                int index = error.Message.IndexOf(' ');
                string percentString = error.Message[..index];
                int percent = int.Parse(percentString);
                int remainingPercent = 100 - percent;
                long estimated = (long)Math.Round(100.0 / percent * elapsed);
                long estimatedRemaining = remainingPercent * elapsed / percent;
                InfoMessage infoMessage = new InfoMessage(localTime, InfoMessageSeverity.Verbose, null,
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