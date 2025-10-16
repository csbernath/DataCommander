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
        var commandText = @$"select proname
from pg_proc
where prokind = 'p' and pronamespace = {schemaNode.Oid}
order by 1";
        var procedureNodes = await Db.ExecuteReaderAsync(
            schemaNode.SchemaCollectionNode.DatabaseNode.CreateConnection,
            new ExecuteReaderRequest(commandText),
            128,
            dataRecord =>
            {
                var name = dataRecord.GetString(0);
                return new ProcedureNode(schemaNode, name);
            },
            cancellationToken);

        return procedureNodes;
    }
}