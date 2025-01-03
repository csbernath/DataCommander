using System;
using System.Data;
using System.Data.SqlServerCe;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api.Connection;

namespace DataCommander.Providers.SqlServerCe40;

internal sealed class Connection : ConnectionBase
{
    private readonly SqlCeConnection _sqlCeConnection;

    public Connection(string connectionString)
    {
        _sqlCeConnection = new SqlCeConnection(connectionString);
        Connection = _sqlCeConnection;
    }

    public override Task OpenAsync(CancellationToken cancellationToken) => _sqlCeConnection.OpenAsync(cancellationToken);

    public override IDbCommand CreateCommand() => _sqlCeConnection.CreateCommand();

    public override string ConnectionName { get; set; }
    public override string Caption => _sqlCeConnection.DataSource;
    public override string DataSource => _sqlCeConnection.DataSource;
    protected void SetDatabase(string database) => throw new NotImplementedException();
    public override string ServerVersion => _sqlCeConnection.ServerVersion;
    public override int TransactionCount => 0;
}