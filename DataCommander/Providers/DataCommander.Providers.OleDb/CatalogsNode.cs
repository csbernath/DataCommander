using System.Collections.Generic;
using System.Data.OleDb;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using DataCommander.Api.Connection;

namespace DataCommander.Providers.OleDb;

internal class CatalogsNode : ITreeNode
{
    public readonly ConnectionStringAndCredential ConnectionStringAndCredential;

    public CatalogsNode(ConnectionStringAndCredential connectionStringAndCredential)
    {
        ConnectionStringAndCredential = connectionStringAndCredential;
    }

    public string? Name => "Catalogs";

    public bool IsLeaf => false;

    async Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        ITreeNode[] treeNodes;

        await using (var connection = ConnectionFactory.CreateConnection(ConnectionStringAndCredential))
        {
            await connection.OpenAsync(cancellationToken);
            var dataTable = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Catalogs, null)!;
            var count = dataTable.Rows.Count;
            var nameColumn = dataTable.Columns["CATALOG_NAME"];
            treeNodes = new ITreeNode[count];

            for (var i = 0; i < count; i++)
            {
                var name = (string)dataTable.Rows[i][nameColumn];
                treeNodes[i] = new CatalogNode(this, name);
            }
        }

        return treeNodes;
    }

    public bool Sortable => false;
    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken) => Task.FromResult<string?>(null);

    public ContextMenu? GetContextMenu() => throw new System.NotImplementedException();
}