using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api.Connection;
using Oracle.ManagedDataAccess.Client;

namespace DataCommander.Providers.Odp;

internal sealed class Connection : ConnectionBase
{
    private readonly OracleConnectionStringBuilder _oracleConnectionStringBuilder;
    private readonly OracleConnection _oracleConnection;
    private string _connectionName;

    public Connection(string connectionString)
    {
        _oracleConnectionStringBuilder = new OracleConnectionStringBuilder(connectionString);            
        _oracleConnection = new OracleConnection(connectionString);
        Connection = _oracleConnection;
        _oracleConnection.InfoMessage += OnInfoMessage;
    }

    public override async Task OpenAsync(CancellationToken cancellationToken)
    {
        await _oracleConnection.OpenAsync(cancellationToken);
        var enlist = bool.Parse(_oracleConnectionStringBuilder.Enlist);
    }

    public override string Caption
    {
        get
        {
            var caption = $"{_oracleConnectionStringBuilder.UserID}@{_oracleConnectionStringBuilder.DataSource}";
            return caption;
        }
    }

    private void OnInfoMessage(object sender, OracleInfoMessageEventArgs e)
    {
        var sb = new StringBuilder();
        sb.AppendLine(e.Message);
        sb.Append("Source: ");
        sb.Append(e.Source);

        InvokeInfoMessage(new[] {InfoMessageFactory.Create(InfoMessageSeverity.Information, null, sb.ToString())});
    }

    public override string ConnectionName
    {
        get => _connectionName;
        set => _connectionName = value;
    }

    public override string DataSource => _oracleConnection.DataSource;

    public override string ServerVersion => _oracleConnection.ServerVersion;

    public override IDbCommand CreateCommand()
    {
        var command = _oracleConnection.CreateCommand();
        command.InitialLONGFetchSize = 8 * 1024;
        command.FetchSize = 256 * 1024;
        return command;
    }

    protected void SetDatabase(string database)
    {
    }

    public override int TransactionCount => 0;
}