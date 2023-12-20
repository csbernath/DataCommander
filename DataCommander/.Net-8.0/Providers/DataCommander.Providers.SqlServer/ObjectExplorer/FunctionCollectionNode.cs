using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class FunctionCollectionNode : ITreeNode
{
    private readonly DatabaseNode _database;

    public FunctionCollectionNode(DatabaseNode database) => _database = database;

    public string Name => "Functions";
    public bool IsLeaf => false;

    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken) =>
        Task.FromResult<IEnumerable<ITreeNode>>(new ITreeNode[]
        {
            new TableValuedFunctionCollectionNode(_database),
            new ScalarValuedFunctionCollectionNode(_database)
        });

    public bool Sortable => false;
    public string Query => null;

    public ContextMenu? GetContextMenu() => null;
}