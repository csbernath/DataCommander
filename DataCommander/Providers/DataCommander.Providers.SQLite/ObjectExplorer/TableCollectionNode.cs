using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Data;

namespace DataCommander.Providers.SQLite.ObjectExplorer;

internal sealed class TableCollectionNode(DatabaseNode databaseNode) : ITreeNode
{
    #region ITreeNode Members

    string? ITreeNode.Name => "Tables";

    bool ITreeNode.IsLeaf => false;

    public IReadOnlyCollection<string> GetFilterableProperties() => [];

    public async Task<IEnumerable<ITreeNode>> GetChildren(IReadOnlyCollection<FilterCriterion> filterCriteria, bool refresh,
        CancellationToken cancellationToken)
    {
        var commandText = $@"select	name
from
(
	select	name
	from	{databaseNode.Name}.sqlite_master
	where	type	= 'table'
	union
	select	'sqlite_master'
) t
order by name collate nocase";

        var list = await Db.ExecuteReaderAsync(
            () => ConnectionFactory.CreateConnection(databaseNode.DatabaseCollectionNode.ConnectionStringAndCredential),
            new ExecuteReaderRequest(commandText),
            128,
            dataRecord =>
            {
                var name = dataRecord.GetString(0);
                return (ITreeNode)new TableNode(databaseNode, name);
            },
            cancellationToken);
        return list;
    }

    public IReadOnlyCollection<FilterCriterion> GetFilterCriteria() => [];

    bool ITreeNode.Sortable => false;

    public bool DynamicChildCount => true;

    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken) => Task.FromResult<string?>(null);
    public ContextMenu? GetContextMenu() => throw new System.NotImplementedException();

    #endregion
}