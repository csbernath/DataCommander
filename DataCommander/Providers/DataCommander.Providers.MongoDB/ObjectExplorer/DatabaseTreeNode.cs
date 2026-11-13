using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

    public IReadOnlyCollection<string> GetFilterableProperties() => throw new NotImplementedException();

    public async IAsyncEnumerable<ITreeNode> GetChildren(
        IReadOnlyCollection<FilterCriterion> filterCriteria,
        bool refresh,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var connectionString = MongoConnectionStringFactory.Create(mongoObjectExplorer.ConnectionStringAndCredential);
        using var mongoClient = new MongoClient(connectionString);
        var database = mongoClient.GetDatabase(databaseName);
        var cursor = await database.ListCollectionsAsync(null, cancellationToken);

        while (await cursor.MoveNextAsync(cancellationToken))
        {
            foreach (var collection in cursor.Current)
            {
                var collectionName = collection["name"].AsString;
                yield return new CollectionTreeNode(this, collectionName);
            }
        }
    }

    public IReadOnlyCollection<FilterCriterion> GetFilterCriteria() => [];

    public bool Sortable => false;

    public bool DynamicChildCount => false;

    public Task<string?> GetQuery(CancellationToken cancellationToken) => throw new NotImplementedException();

    public ContextMenu? GetContextMenu() => throw new NotImplementedException();
}