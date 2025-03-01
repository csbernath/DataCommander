using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;

namespace DataCommander.Providers.OleDb;

internal class CatalogNode(CatalogsNode catalogsNode, string? name) : ITreeNode
{
    public CatalogsNode CatalogsNode { get; } = catalogsNode;

    string? ITreeNode.Name
    {
        get
        {
            var name = Name;

            if (name == null)
                name = "[No catalogs found]";
            else if (name.Length == 0)
                name = "[Catalog name is empty]";

            return name;
        }
    }

    public bool IsLeaf => false;

    async Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        ITreeNode[] treeNodes;

        try
        {
            DataTable dataTable;
            await using (var connection = ConnectionFactory.CreateConnection(CatalogsNode.ConnectionStringAndCredential))
            {
                await connection.OpenAsync(cancellationToken);
                var restrictions = new object[] { Name };
                dataTable = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Schemata, restrictions)!;
            }
            
            var count = dataTable.Rows.Count;
            var nameColumn = dataTable.Columns["SCHEMA_NAME"];
            treeNodes = new ITreeNode[count];

            for (var i = 0; i < count; i++)
            {
                var schemaName = (string)dataTable.Rows[i][nameColumn];
                treeNodes[i] = new SchemaNode(this, schemaName);
            }
        }
        catch
        {
            treeNodes = new ITreeNode[1];
            treeNodes[0] = new SchemaNode(this, null);
        }

        return treeNodes;
    }

    public bool Sortable => false;
    public string? Query => null;
    public string? Name { get; } = name;

    public ContextMenu? GetContextMenu() => null;
}