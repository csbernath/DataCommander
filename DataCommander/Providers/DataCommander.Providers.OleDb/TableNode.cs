using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Collections.ReadOnly;

namespace DataCommander.Providers.OleDb;

sealed class TableNode(SchemaNode schema, string name) : ITreeNode
{
    string ITreeNode.Name
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
                string name2;

                if (name.IndexOf(' ') >= 0)
                {
                    name2 = "[" + name + "]";
                }
                else
                {
                    name2 = name;
                }

                query = "select * from " + name2;
            }
            else
                query = null;

            return query;
        }
    }

    void Columns_Click(object sender, EventArgs e)
    {
        var restrictions = new object[] { schema.Catalog.Name, schema.Name, name };
        var dataTable = schema.Catalog.Connection.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, restrictions);
        var dataSet = new DataSet();
        dataSet.Tables.Add(dataTable);

        var queryForm = (IQueryForm)sender;
        queryForm.ShowDataSet(dataSet);
    }

    public ContextMenu? GetContextMenu()
    {
        var menuItem = new MenuItem("Columns", Columns_Click, EmptyReadOnlyCollection<MenuItem>.Value);
        var items = new[] { menuItem }.ToReadOnlyCollection();
        var contextMenu = new ContextMenu(items);
        return contextMenu;
    }
}