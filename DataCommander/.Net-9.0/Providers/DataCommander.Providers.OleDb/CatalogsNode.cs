using System.Collections.Generic;
using System.Data.OleDb;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;

namespace DataCommander.Providers.OleDb;

internal class CatalogsNode(OleDbConnection connection) : ITreeNode
{
    public string? Name => "Catalogs";

    public bool IsLeaf => false;

    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        ITreeNode[] treeNodes;

        try
        {
            System.Data.DataTable? dataTable = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Catalogs, null);
            int count = dataTable.Rows.Count;
            System.Data.DataColumn? nameColumn = dataTable.Columns["CATALOG_NAME"];
            treeNodes = new ITreeNode[count];

            for (int i = 0; i < count; i++)
            {
                string name = (string)dataTable.Rows[i][nameColumn];
                treeNodes[i] = new CatalogNode(connection, name);
            }
        }
        catch
        {
            treeNodes = new ITreeNode[1];
            treeNodes[0] = new CatalogNode(connection, null);
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