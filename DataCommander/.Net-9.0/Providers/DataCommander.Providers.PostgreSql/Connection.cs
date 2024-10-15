using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api.Connection;
using Npgsql;

namespace DataCommander.Providers.PostgreSql;

internal sealed class Connection : ConnectionBase
{
    private readonly ConnectionStringAndCredential _connectionStringAndCredential;
    private NpgsqlConnection _npgsqlConnection;

    public Connection(ConnectionStringAndCredential connectionStringAndCredential)
    {
        _connectionStringAndCredential = connectionStringAndCredential;
        CreateConnection();
    }

    public override Task OpenAsync(CancellationToken cancellationToken) => _npgsqlConnection.OpenAsync(cancellationToken);
    public override DbCommand CreateCommand() => _npgsqlConnection.CreateCommand();
    public override string ConnectionName { get; set; }
    public override string Caption => _npgsqlConnection.Database;
    public override string DataSource => _npgsqlConnection.DataSource;
    public override string ServerVersion => _npgsqlConnection.ServerVersion;

    public override string ConnectionInformation => null;

    public override Task<int> GetTransactionCountAsync(CancellationToken cancellationToken) => Task.FromResult(0);

    private void CreateConnection()
    {
        var npgsqlConnectionStringBuilder = new NpgsqlConnectionStringBuilder(_connectionStringAndCredential.ConnectionString)
        {
            ApplicationName = "Data Commander",
            Pooling = false
        };

        var credential = _connectionStringAndCredential.Credential;
        if (credential != null)
        {
            npgsqlConnectionStringBuilder.Username = credential.UserId;
            npgsqlConnectionStringBuilder.Password = PasswordFactory.Unprotect(credential.Password.Protected);
        }

        _npgsqlConnection = new NpgsqlConnection(npgsqlConnectionStringBuilder.ConnectionString);
        Connection = _npgsqlConnection;
    }
}