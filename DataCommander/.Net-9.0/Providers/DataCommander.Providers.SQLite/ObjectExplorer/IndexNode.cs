using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Data;

namespace DataCommander.Providers.SQLite.ObjectExplorer;

internal sealed class IndexNode : ITreeNode
{
    private readonly TableNode _tableNode;
    private readonly string? _name;

    public IndexNode(TableNode tableNode, string? name)
    {
        _tableNode = tableNode;
        _name = name;
    }

    #region ITreeNode Members

    string? ITreeNode.Name => _name;

    bool ITreeNode.IsLeaf => true;

    public Task<IEnumerable<ITreeNode>> GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        return null;
    }

    bool ITreeNode.Sortable => false;

    string ITreeNode.Query
    {
        get
        {
            var commandText = $@"select sql
from main.sqlite_master
where
    type = 'index'
    and name = '{_name}'";
            var scalar = Db.ExecuteScalar(
                () => ConnectionFactory.CreateConnection(_tableNode.DatabaseNode.DatabaseCollectionNode.ConnectionStringAndCredential),
                new CreateCommandRequest(commandText));
            var sql = (string)scalar;
            return sql;
        }
    }

    public ContextMenu? GetContextMenu()
    {
        throw new System.NotImplementedException();
    }

    #endregion
}