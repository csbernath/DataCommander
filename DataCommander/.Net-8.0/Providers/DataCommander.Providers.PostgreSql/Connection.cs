using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api.Connection;
using Npgsql;

namespace DataCommander.Providers.PostgreSql;

internal sealed class Connection : ConnectionBase
{
    #region Private Fields

    private readonly ConnectionStringAndCredential _connectionStringAndCredential;
    private readonly NpgsqlConnection _npgsqlConnection;

    #endregion

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

    public override string ConnectionInformation => throw new System.NotImplementedException();

    public override Task<int> GetTransactionCountAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(0);
    }
    
    private void CreateConnection()
    {
        var sqlConnectionStringBuilder = new NpgsqlConnectionStringBuilder(_connectionStringAndCredential.ConnectionString)
        {
            ApplicationName = "Data Commander",
            Pooling = false
        };

        var credential = _connectionStringAndCredential.Credential;
        if (credential != null)
        {
            sqlConnectionStringBuilder.Username = credential.UserId;
            sqlConnectionStringBuilder.Password = credential.Password.SecureString

        }
            sqlCredential = new SqlCredential(credential.UserId, credential.Password.SecureString);

        _npgsqlConnection = new NpgsqlConnection(sqlConnectionStringBuilder.ConnectionString);
        Connection = _sqlConnection;
        _sqlConnection.FireInfoMessageEventOnUserErrors = true;
        _sqlConnection.InfoMessage += OnInfoMessage;
        _sqlConnection.StateChange += OnStateChange;
    }
}