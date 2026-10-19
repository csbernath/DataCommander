using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using DataCommander.Api.Connection;
using MongoDB.Driver;

namespace DataCommander.Providers.MongoDB.ObjectExplorer;

public class MongoObjectExplorer : IObjectExplorer
{
    private ConnectionStringAndCredential _connectionStringAndCredential;
    private string? _currentDatabaseName;
    private string?_currentCollectionName;

    public ConnectionStringAndCredential ConnectionStringAndCredential => _connectionStringAndCredential;

    public void SetCurrentDatabaseAndCollectionName(string currentDatabaseName, string currentCollectionName)
    {
        _currentDatabaseName = currentDatabaseName;
        _currentCollectionName = currentCollectionName;
    }

    public bool Sortable => false;

    public void SetConnectionStringAndCredential(string? connectionName, ConnectionStringAndCredential connectionStringAndCredential)
    {
        _connectionStringAndCredential = connectionStringAndCredential;
    }

    public async Task<IEnumerable<ITreeNode>> GetChildren(IReadOnlyList<FilterCriterion> filterCriteria, bool refresh, CancellationToken cancellationToken)
    {
        using var mongoClient = new MongoClient(_connectionStringAndCredential.ConnectionString);
        var listDatabasesAsyncCursor = await mongoClient.ListDatabasesAsync(cancellationToken);
        var databases = await listDatabasesAsyncCursor.ToListAsync(cancellationToken);
        ITreeNode[] children =
        [
            .. databases
                .Select(database =>
                {
                    var name = database["name"].AsString;
                    return new DatabaseTreeNode(this, name);
                })
        ];
        return children;
    }

    public void InitializeCommand(IDbCommand command)
    {
        if (_currentDatabaseName != null && _currentCollectionName != null)
        {
            var mongoDbCommand = (MongoCommand)command;
            mongoDbCommand.SetDatabaseAndCollectionName(_currentDatabaseName, _currentCollectionName);
        }
    }
}