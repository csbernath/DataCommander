using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;

namespace DataCommander.Providers.SQLite.ObjectExplorer;

sealed class DatabaseNode : ITreeNode
{
    private readonly DatabaseCollectionNode _databaseCollectionNode;

    public DatabaseNode(DatabaseCollectionNode databaseCollectionNode, string name)
    {
        _databaseCollectionNode = databaseCollectionNode;
        Name = name;
    }

    public DatabaseCollectionNode DatabaseCollectionNode => _databaseCollectionNode;

    #region ITreeNode Members
    public string Name { get; }

    bool ITreeNode.IsLeaf => false;

    public Task<IEnumerable<ITreeNode>> GetChildren(bool refresh, CancellationToken cancellationToken) =>
        Task.FromResult<IEnumerable<ITreeNode>>(new ITreeNode[]
        {
            new TableCollectionNode(this)
        });

    bool ITreeNode.Sortable => false;

    string ITreeNode.Query => null;

    public ContextMenu? GetContextMenu()
    {
        throw new System.NotImplementedException();
    }

    #endregion
}