using System.Collections.Generic;
using System.Data.OleDb;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;

namespace DataCommander.Providers.OleDb;

internal sealed class TableCollectionNode(SchemaNode schema) : ITreeNode
{
    public string? Name => "Tables";

    public bool IsLeaf => false;

    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        ITreeNode[] treeNodes;

        try
        {
            object[] restrictions = null;
            var catalog = schema.Catalog.Name;

            if (catalog != null)
                restrictions = [catalog, schema.Name];

            var dataTable = schema.Catalog.Connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, restrictions)!;
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

        return Task.FromResult<IEnumerable<ITreeNode>>(treeNodes);
    }

    public bool Sortable => false;
    public string? Query => null;

    public ContextMenu? GetContextMenu() => throw new System.NotImplementedException();
}