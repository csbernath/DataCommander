﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using DataCommander.Api;
using Foundation.Collections.ReadOnly;

namespace DataCommander.Providers.OleDb;

sealed class TableNode : ITreeNode
{
    private readonly SchemaNode schema;
    private readonly string name;

    public TableNode(SchemaNode schema, string name)
    {
        this.schema = schema;
        this.name = name;
    }

    string ITreeNode.Name
    {
        get
        {
            var name = this.name;

            if (name == null)
                name = "[No tables found]";

            return name;
        }
    }

    public bool IsLeaf => true;

    public IEnumerable<ITreeNode> GetChildren(bool refresh)
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