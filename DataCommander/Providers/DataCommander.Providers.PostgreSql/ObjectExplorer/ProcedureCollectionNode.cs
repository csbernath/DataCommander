using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Data;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer;

internal sealed class ProcedureCollectionNode(SchemaNode schemaNode) : ITreeNode
{
    bool ITreeNode.IsLeaf => false;
    string ITreeNode.Name => "Procedure";
    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken) => Task.FromResult<string?>(null);
    public ContextMenu? GetContextMenu() => null;

    bool ITreeNode.Sortable => false;

    public async Task<IEnumerable<ITreeNode>> GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        var commandText = @$"select
    proname,
	proargnames
from pg_proc
where
    prokind = 'p' and
    pronamespace = {schemaNode.Oid}
order by proname";
        var procedureNodes = await Db.ExecuteReaderAsync(
            schemaNode.SchemaCollectionNode.DatabaseNode.CreateConnection,
            new ExecuteReaderRequest(commandText),
            128,
            dataRecord =>
            {
                var name = dataRecord.GetString(0);
                var argnames = dataRecord.IsDBNull(1)
                    ? null
                    : (string[])dataRecord.GetValue(1);
                return new ProcedureNode(schemaNode, name, argnames);
            },
            cancellationToken);

        return procedureNodes;
    }
}