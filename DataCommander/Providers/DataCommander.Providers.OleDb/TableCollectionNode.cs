using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;

namespace DataCommander.Providers.OleDb;

internal sealed class TableCollectionNode(SchemaNode schema) : ITreeNode
{
    public string? Name => "Tables";

    public bool IsLeaf => false;

    async Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        ITreeNode[] treeNodes;

        try
        {
            object[] restrictions = null;
            var catalog = schema.Catalog.Name;

            if (catalog != null)
                restrictions = [catalog, schema.Name];

            DataTable dataTable;
            await using (var connection = ConnectionFactory.CreateConnection(schema.Catalog.CatalogsNode.ConnectionStringAndCredential))
            {
                await connection.OpenAsync(cancellationToken);
                dataTable = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, restrictions)!;
            }

            var count = dataTable.Rows.Count;
            var nameColumn = dataTable.Columns["TABLE_NAME"];
            treeNodes = new ITreeNode[count];

            for (var i = 0; i < count; i++)
            {
                var name = (string)dataTable.Rows[i][nameColumn];
                treeNodes[i] = new TableNode(schema, name);
            }
        }
        catch
        {
            treeNodes = [new TableNode(schema, null)];
        }

        return treeNodes;
    }

    public bool Sortable => false;
    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken) => Task.FromResult<string?>(null);

    public ContextMenu? GetContextMenu() => throw new System.NotImplementedException();
}