using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;

namespace DataCommander.Providers.SQLite.ObjectExplorer;

sealed class DatabaseNode(DatabaseCollectionNode databaseCollectionNode, string? name) : ITreeNode
{
    private readonly DatabaseCollectionNode _databaseCollectionNode = databaseCollectionNode;

    public DatabaseCollectionNode DatabaseCollectionNode => _databaseCollectionNode;

    #region ITreeNode Members
    public string? Name { get; } = name;

    bool ITreeNode.IsLeaf => false;

    public Task<IEnumerable<ITreeNode>> GetChildren(bool refresh, CancellationToken cancellationToken) =>
        Task.FromResult<IEnumerable<ITreeNode>>(
        [
            new TableCollectionNode(this)
        ]);

    bool ITreeNode.Sortable => false;
    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken) => Task.FromResult<string?>(null);
    public ContextMenu? GetContextMenu() => throw new System.NotImplementedException();

    #endregion
}