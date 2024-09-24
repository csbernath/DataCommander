using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Data;

namespace DataCommander.Providers.SQLite.ObjectExplorer;

internal sealed class TableCollectionNode(DatabaseNode databaseNode) : ITreeNode
{
    private readonly DatabaseNode _databaseNode = databaseNode;

    #region ITreeNode Members

    string? ITreeNode.Name => "Tables";

    bool ITreeNode.IsLeaf => false;

    public async Task<IEnumerable<ITreeNode>> GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        var commandText = $@"select	name
from
(
	select	name
	from	{_databaseNode.Name}.sqlite_master
	where	type	= 'table'
	union
	select	'sqlite_master'
) t
order by name collate nocase";

        Foundation.Collections.ReadOnly.ReadOnlySegmentLinkedList<ITreeNode> list = await Db.ExecuteReaderAsync(
            () => ConnectionFactory.CreateConnection(_databaseNode.DatabaseCollectionNode.ConnectionStringAndCredential),
            new ExecuteReaderRequest(commandText),
            128,
            dataRecord =>
            {
                var name = dataRecord.GetString(0);
                return (ITreeNode)new TableNode(_databaseNode, name);
            },
            cancellationToken);
        return list;
    }

    bool ITreeNode.Sortable => false;
    string ITreeNode.Query => null;

    public ContextMenu? GetContextMenu() => throw new System.NotImplementedException();

    #endregion
}