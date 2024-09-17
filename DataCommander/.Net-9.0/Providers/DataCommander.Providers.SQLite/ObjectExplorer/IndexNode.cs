using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Data;

namespace DataCommander.Providers.SQLite.ObjectExplorer;

internal sealed class IndexNode(TableNode tableNode, string? name) : ITreeNode
{
    private readonly TableNode _tableNode = tableNode;
    private readonly string? _name = name;

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
            string commandText = $@"select sql
from main.sqlite_master
where
    type = 'index'
    and name = '{_name}'";
            object scalar = Db.ExecuteScalar(
                () => ConnectionFactory.CreateConnection(_tableNode.DatabaseNode.DatabaseCollectionNode.ConnectionStringAndCredential),
                new CreateCommandRequest(commandText));
            string sql = (string)scalar;
            return sql;
        }
    }

    public ContextMenu? GetContextMenu()
    {
        throw new System.NotImplementedException();
    }

    #endregion
}