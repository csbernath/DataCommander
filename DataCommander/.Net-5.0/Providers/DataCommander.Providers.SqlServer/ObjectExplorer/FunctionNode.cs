using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using DataCommander.Providers.Query;
using Foundation.Collections.ReadOnly;
using Foundation.Data.SqlClient;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class FunctionNode : ITreeNode
{
    private readonly DatabaseNode _database;
    private readonly string _name;
    private readonly string _owner;
    private readonly string _xtype;

    public FunctionNode(
        DatabaseNode database,
        string owner,
        string name,
        string xtype)
    {
        _database = database;
        _owner = owner;
        _name = name;
        _xtype = xtype;
    }

    public string Name => $"{_owner}.{_name}";

    public bool IsLeaf => true;

    IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
    {
        return null;
    }

    public bool Sortable => false;

    public string Query
    {
        get
        {
            //string query = string.Format("select {0}.{1}.[{2}]()",database.Name,owner,name);
            string query;

            switch (_xtype)
            {
                case "FN": //Scalar function
                    query = $"select {_database.Name}.{_owner}.[{_name}]()";
                    break;

                case "TF": //Table function
                case "IF": //Inlined table-function
                    query = $@"select	*
from	{_database.Name}.{_owner}.[{_name}]()";
                    break;

                default:
                    query = null;
                    break;
            }

            return query;
        }
    }

    public ContextMenu GetContextMenu()
    {
        var scriptObjectMenuItem = new MenuItem("Script Object", menuItemScriptObject_Click, EmptyReadOnlyCollection<MenuItem>.Value);
        var menuItems = new[] { scriptObjectMenuItem }.ToReadOnlyCollection();
        var contextMenu = new ContextMenu(menuItems);
        return contextMenu;
    }

    private void menuItemScriptObject_Click(object sender, EventArgs e)
    {
        var connectionString = _database.Databases.Server.ConnectionString;
        string text;
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            text = SqlDatabase.GetSysComments(connection, _database.Name, _owner, _name);
        }

        QueryForm.ShowText(text);
    }
}