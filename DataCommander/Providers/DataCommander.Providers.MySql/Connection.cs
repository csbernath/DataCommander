using DataCommander.Api.Connection;
using MySql.Data.MySqlClient;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace DataCommander.Providers.MySql;

internal sealed class Connection : ConnectionBase
{
    private readonly string _connectionString;
    private readonly MySqlConnection _mySqlConnection;
    private string _connectionName;

    public Connection(string connectionString)
    {
        _connectionString = connectionString;
        _mySqlConnection = new MySqlConnection(connectionString);
        SetConnection(_mySqlConnection);
    }

    public override Task OpenAsync(CancellationToken cancellationToken) => _mySqlConnection.OpenAsync(cancellationToken);
    public override DbCommand CreateCommand() => _mySqlConnection.CreateCommand();

    public override string DataSource
    {
        get
        {
            // TODO
            var csb = new MySqlConnectionStringBuilder(_connectionString);
            return csb.Database;
        }
    }

    protected void SetDatabase(string database)
    {
        throw new NotImplementedException();
    }

    public override string ServerVersion => _mySqlConnection.ServerVersion;
    public override string? ConnectionInformation { get; }

    public override Task<int> GetTransactionCountAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(0);
    }
}