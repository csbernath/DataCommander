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
            string? name1 = name;

            if (name1 == null)
                name1 = "[No tables found]";

            return name1;
        }
    }

    public bool IsLeaf => true;

    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        return null;
    }

    public bool Sortable => false;

    public string Query
    {
        get
        {
            string query;

            if (name != null)
            {
                string name2 = name.IndexOf(' ') >= 0
                    ? "[" + name + "]"
                    : name;
                query = "select * from " + name2;
            }
            else
                query = null;

            return query;
        }
    }

    public ContextMenu? GetContextMenu()
    {
        MenuItem menuItem = new MenuItem("Columns", Columns_Click, []);
        System.Collections.ObjectModel.ReadOnlyCollection<MenuItem> items = new[] { menuItem }.ToReadOnlyCollection();
        ContextMenu contextMenu = new ContextMenu(items);
        return contextMenu;
    }

    private void Columns_Click(object sender, EventArgs e)
    {
        object[] restrictions = new object[] { schema.Catalog.Name, schema.Name, name };
        DataTable? dataTable = schema.Catalog.Connection.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, restrictions);
        DataSet dataSet = new DataSet();
        dataSet.Tables.Add(dataTable);

        IQueryForm queryForm = (IQueryForm)sender;
        queryForm.ShowDataSet(dataSet);
    }
}