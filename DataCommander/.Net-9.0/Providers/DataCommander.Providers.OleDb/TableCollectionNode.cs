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

    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        ITreeNode[] treeNodes;

        try
        {
            object[] restrictions = null;
            string? catalog = schema.Catalog.Name;

            if (catalog != null)
                restrictions = [catalog, schema.Name];

            DataTable dataTable = dataTable = schema.Catalog.Connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, restrictions);
            int count = dataTable.Rows.Count;
            DataColumn? nameColumn = dataTable.Columns["TABLE_NAME"];
            treeNodes = new ITreeNode[count];

            for (int i = 0; i < count; i++)
            {
                string name = (string)dataTable.Rows[i][nameColumn];
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
    public string Query => null;

    public ContextMenu? GetContextMenu()
    {
        throw new System.NotImplementedException();
    }
}