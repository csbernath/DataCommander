using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace DataCommander.Providers.MongoDB;

public class MongoDbCommand(MongoDBConnection connection) : DbCommand
{
    private string _databaseName;
    private string _collectionName;
    private string? _commandText;
    private MongoDbParameterCollection _parameters = new();

    public void SetDatabaseAndCollectionName(string databaseName, string collectionName)
    {
        _databaseName = databaseName;
        _collectionName = collectionName;
    }
    
    public string? DatabaseName => _databaseName;
    
    public string? CollectionName => _collectionName;

    public MongoDBConnection Connection => connection;

    public override void Cancel()
    {
        throw new System.NotImplementedException();
    }

    public override int ExecuteNonQuery() => throw new System.NotImplementedException();

    public override object? ExecuteScalar() => throw new System.NotImplementedException();

    public override void Prepare()
    {
        throw new System.NotImplementedException();
    }

    [AllowNull]
    public override string? CommandText
    {
        get => _commandText;
        set => _commandText = value;
    }

    public override int CommandTimeout
    {
        get => throw new System.NotImplementedException();
        set { }
    }

    public override CommandType CommandType
    {
        get => throw new System.NotImplementedException();
        set { }
    }

    public override UpdateRowSource UpdatedRowSource
    {
        get => throw new System.NotImplementedException();
        set => throw new System.NotImplementedException();
    }

    protected override DbConnection? DbConnection
    {
        get => throw new System.NotImplementedException();
        set => throw new System.NotImplementedException();
    }

    protected override DbParameterCollection DbParameterCollection => _parameters;

    protected override DbTransaction? DbTransaction
    {
        get => throw new System.NotImplementedException();
        set { }
    }

    public override bool DesignTimeVisible
    {
        get => throw new System.NotImplementedException();
        set => throw new System.NotImplementedException();
    }

    protected override DbParameter CreateDbParameter() => throw new System.NotImplementedException();

    protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior) => throw new System.NotImplementedException();

    protected override async Task<DbDataReader> ExecuteDbDataReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken)
    {
        var mongoDbDataReader = new MongoDbDataReader(this);
        await mongoDbDataReader.Execute(cancellationToken);
        return mongoDbDataReader;
    }
}