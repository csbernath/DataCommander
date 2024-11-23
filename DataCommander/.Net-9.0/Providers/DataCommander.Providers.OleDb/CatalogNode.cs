using System.Collections.Generic;
using System.Data.OleDb;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;

namespace DataCommander.Providers.OleDb;

internal class CatalogNode(OleDbConnection connection, string? name) : ITreeNode
{
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

    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        ITreeNode[] treeNodes;

        try
        {
            var restrictions = new object[] { Name };
            var dataTable = Connection.GetOleDbSchemaTable(OleDbSchemaGuid.Schemata, restrictions)!;
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

        return Task.FromResult<IEnumerable<ITreeNode>>(treeNodes);
    }

    public bool Sortable => false;
    public string? Query => null;
    public OleDbConnection Connection { get; } = connection;
    public string? Name { get; } = name;

    public ContextMenu? GetContextMenu() => null;
}