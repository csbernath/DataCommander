using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;

namespace DataCommander.Providers.OleDb;

internal sealed class ProcedureCollectionNode(SchemaNode schema) : ITreeNode
{
    public string Name => "Procedures";
    public bool IsLeaf => false;

    async Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        var restrictions = new object[] { schema.Catalog.Name!, schema.Name };
        DataTable dataTable;

        await using (var connection = ConnectionFactory.CreateConnection(schema.Catalog.CatalogsNode.ConnectionStringAndCredential))
        {
            await connection.OpenAsync(cancellationToken);
            dataTable = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Procedures, restrictions)!;
        }

        var count = dataTable.Rows.Count;
        var procedureName = dataTable.Columns["PROCEDURE_NAME"]!;
        var treeNodes = new ITreeNode[count];
        for (var i = 0; i < count; i++)
        {
            var name = (string)dataTable.Rows[i][procedureName];
            treeNodes[i] = new ProcedureNode(name);
        }

        return treeNodes;
    }

    public bool Sortable => false;
    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken) => Task.FromResult<string?>(null);

    public ContextMenu? GetContextMenu() => throw new System.NotImplementedException();
}