using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace DataCommander.Providers.MongoDB;

public class MongoCommand(MongoConnection connection) : DbCommand
{
    private string _databaseName;
    private string _collectionName;
    private string? _commandText;
    private MongoParameterCollection _parameters = new();

    public void SetDatabaseAndCollectionName(string databaseName, string collectionName)
    {
        _databaseName = databaseName;
        _collectionName = collectionName;
    }
    
    public string? DatabaseName => _databaseName;
    
    public string? CollectionName => _collectionName;

    public MongoConnection Connection => connection;

    public override void Cancel()
    {
        throw new NotImplementedException();
    }

    public override int ExecuteNonQuery() => throw new NotImplementedException();

    public override object? ExecuteScalar() => throw new NotImplementedException();

    public override void Prepare()
    {
        throw new NotImplementedException();
    }

    [AllowNull]
    public override string? CommandText
    {
        get => _commandText;
        set => _commandText = value;
    }

    public override int CommandTimeout
    {
        get => throw new NotImplementedException();
        set { }
    }

    public override CommandType CommandType
    {
        get => throw new NotImplementedException();
        set { }
    }

    public override UpdateRowSource UpdatedRowSource
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    protected override DbConnection? DbConnection
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    protected override DbParameterCollection DbParameterCollection => _parameters;

    protected override DbTransaction? DbTransaction
    {
        get => throw new NotImplementedException();
        set { }
    }

    public override bool DesignTimeVisible
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    protected override DbParameter CreateDbParameter() => throw new NotImplementedException();

    protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior) => throw new NotImplementedException();

    protected override async Task<DbDataReader> ExecuteDbDataReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken)
    {
        var mongoDbDataReader = new MongoDataReader(this);
        await mongoDbDataReader.Execute(cancellationToken);
        return mongoDbDataReader;
    }
}