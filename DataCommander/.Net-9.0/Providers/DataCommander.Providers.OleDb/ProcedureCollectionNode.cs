using System.Collections.Generic;
using System.Data.OleDb;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;

namespace DataCommander.Providers.OleDb;

internal sealed class ProcedureCollectionNode(SchemaNode schema) : ITreeNode
{
    public string? Name => "Procedures";

    public bool IsLeaf => false;

    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        ITreeNode[] treeNodes;

        try
        {
            object[] restrictions = new object[] { schema.Catalog.Name, schema.Name };
            System.Data.DataTable? dataTable = schema.Catalog.Connection.GetOleDbSchemaTable(OleDbSchemaGuid.Procedures,
                restrictions);
            int count = dataTable.Rows.Count;
            System.Data.DataColumn? procedureName = dataTable.Columns["PROCEDURE_NAME"];
            treeNodes = new ITreeNode[count];

            for (int i = 0; i < count; i++)
            {
                string name = (string)dataTable.Rows[i][procedureName];
                treeNodes[i] = new ProcedureNode(name);
            }
        }
        catch
        {
            treeNodes = [new ProcedureNode(null)];
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