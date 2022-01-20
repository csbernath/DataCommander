using System.Collections.Generic;
using System.Data.OleDb;
using DataCommander.Api;

namespace DataCommander.Providers.OleDb;

internal sealed class ProcedureCollectionNode : ITreeNode
{
    private readonly SchemaNode schema;

    public ProcedureCollectionNode(SchemaNode schema)
    {
        this.schema = schema;
    }

    public string Name => "Procedures";

    public bool IsLeaf => false;

    public IEnumerable<ITreeNode> GetChildren(bool refresh)
    {
        ITreeNode[] treeNodes;

        try
        {
            var restrictions = new object[] { schema.Catalog.Name, schema.Name };
            var dataTable = schema.Catalog.Connection.GetOleDbSchemaTable(OleDbSchemaGuid.Procedures,
                restrictions);
            var count = dataTable.Rows.Count;
            var procedureName = dataTable.Columns["PROCEDURE_NAME"];
            treeNodes = new ITreeNode[count];

            for (var i = 0; i < count; i++)
            {
                var name = (string)dataTable.Rows[i][procedureName];
                treeNodes[i] = new ProcedureNode(name);
            }
        }
        catch
        {
            treeNodes = new ITreeNode[] { new ProcedureNode(null) };
        }

        return treeNodes;
    }

    public bool Sortable => false;
    public string Query => null;

    public ContextMenu? GetContextMenu()
    {
        throw new System.NotImplementedException();
    }
}