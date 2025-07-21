using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Collections.ReadOnly;

namespace DataCommander.Providers.OleDb;

sealed class TableNode(SchemaNode schema, string? name) : ITreeNode
{
    public string? Name
    {
        get
        {
            var name1 = name;

            if (name1 == null)
                name1 = "[No tables found]";

            return name1;
        }
    }

    public bool IsLeaf => true;

    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken) =>
        Task.FromResult<IEnumerable<ITreeNode>>(Array.Empty<ITreeNode>());

    public bool Sortable => false;

    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken)
    {
        string? query;

        if (name != null)
        {
            var name2 = name.Contains(' ')
                ? "[" + name + "]"
                : name;
            query = "select * from " + name2;
        }
        else
            query = null;

        return Task.FromResult(query);
    }

    public ContextMenu? GetContextMenu()
    {
        var menuItem = new MenuItem("Columns", Columns_Click, []);
        var items = new[] { menuItem }.ToReadOnlyCollection();
        var contextMenu = new ContextMenu(items);
        return contextMenu;
    }

    private void Columns_Click(object? sender, EventArgs e)
    {
        DataTable dataTable;
        using (var connection = ConnectionFactory.CreateConnection(schema.Catalog.CatalogsNode.ConnectionStringAndCredential))
        {
            connection.Open();
            var restrictions = new object[] { schema.Catalog.Name, schema.Name, name };
            dataTable = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, restrictions);
        }

        var dataSet = new DataSet();
        dataSet.Tables.Add(dataTable);
        
        var queryForm = (IQueryForm)sender!;
        queryForm.ShowDataSet(dataSet);
    }
}