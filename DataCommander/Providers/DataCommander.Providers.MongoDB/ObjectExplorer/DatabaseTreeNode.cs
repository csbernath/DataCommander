using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using MongoDB.Driver;

namespace DataCommander.Providers.MongoDB.ObjectExplorer;

public class DatabaseTreeNode(MongoObjectExplorer mongoObjectExplorer, string databaseName)
    : ITreeNode
{
    public MongoObjectExplorer MongoObjectExplorer => mongoObjectExplorer;
    
    public string Name => databaseName;

    public bool IsLeaf => false;

    public IReadOnlyCollection<string> GetFilterableProperties() => throw new System.NotImplementedException();

    public async Task<IEnumerable<ITreeNode>> GetChildren(
        IReadOnlyCollection<FilterCriterion> filterCriteria,
        bool refresh,
        CancellationToken cancellationToken)
    {
        using var mongoClient = new MongoClient(mongoObjectExplorer.ConnectionStringAndCredential.ConnectionString);
        var database = mongoClient.GetDatabase(databaseName);
        var cursor = await database.ListCollectionsAsync(null, cancellationToken);
        var collections = await cursor.ToListAsync(cancellationToken);
        ITreeNode[] children =
        [
            .. collections.Select(collection =>
            {
                var collectionName = collection["name"].AsString;
                return new CollectionTreeNode(this, collectionName);
            })
        ];
        return children;
    }

    public IReadOnlyCollection<FilterCriterion> GetFilterCriteria() => [];

    public bool Sortable => false;

    public bool DynamicChildCount => false;

    public Task<string?> GetQuery(CancellationToken cancellationToken) => throw new System.NotImplementedException();

    public ContextMenu? GetContextMenu() => throw new System.NotImplementedException();
}