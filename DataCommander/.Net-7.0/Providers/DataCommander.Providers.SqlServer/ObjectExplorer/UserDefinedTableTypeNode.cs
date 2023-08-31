using System;
using System.Collections.Generic;
using System.Text;
using DataCommander.Api;
using Foundation.Collections.ReadOnly;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class UserDefinedTableTypeNode : ITreeNode
{
    private readonly DatabaseNode _database;
    private readonly int _id;
    private readonly string _name;
    private readonly string _schema;

    public UserDefinedTableTypeNode(DatabaseNode database, int id, string schema, string name)
    {
        _database = database;
        _id = id;
        _schema = schema;
        _name = name;
    }

    string ITreeNode.Name => $"{_schema}.{_name}";
    bool ITreeNode.IsLeaf => false;

    IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
    {
        return new ITreeNode[]
        {
            new ColumnCollectionNode(_database, _id)
        };
    }

    bool ITreeNode.Sortable => false;
    string ITreeNode.Query => null;

    public ContextMenu? GetContextMenu()
    {
        var menuItems = new MenuItem[]
        {
            new("Script", Script_OnClick, EmptyReadOnlyCollection<MenuItem>.Value)
        }.ToReadOnlyCollection();

        var contextMenu = new ContextMenu(menuItems);
        return contextMenu;
    }

    private void Script_OnClick(object? sender, EventArgs e)
    {
        var queryForm = (IQueryForm)sender;
        var connectionInfo = SqlObjectScripter.CreateSqlConnectionInfo(_database.Databases.Server.ConnectionString);
        var connection = new ServerConnection(connectionInfo);
        connection.Connect();
        var server = new Server(connection);
        var database = server.Databases[_database.Name];
        var userDefinedTableType = database.UserDefinedTableTypes[_name, _schema];
        var stringCollection = userDefinedTableType.Script();

        var sb = new StringBuilder();
        var first = true;
        foreach (var s in stringCollection)
        {
            if (first)
                first = false;
            else
                sb.AppendLine("GO");

            sb.AppendLine(s);
        }

        queryForm.SetClipboardText(sb.ToString());
    }
}