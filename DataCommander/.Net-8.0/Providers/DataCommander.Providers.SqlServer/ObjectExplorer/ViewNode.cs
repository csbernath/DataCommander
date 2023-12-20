using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Microsoft.Data.SqlClient;
using Foundation.Collections.ReadOnly;
using Foundation.Data.SqlClient;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class ViewNode : ITreeNode
{
    private readonly DatabaseNode _database;
    private readonly int _id;
    private readonly string? _name;
    private readonly string? _schema;

    public ViewNode(DatabaseNode database, int id, string? schema, string? name)
    {
        _database = database;
        _id = id;
        _schema = schema;
        _name = name;
    }

    public string Name => $"{_schema}.{_name}";
    public bool IsLeaf => false;

    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        return Task.FromResult<IEnumerable<ITreeNode>>(new ITreeNode[]
        {
            new ColumnCollectionNode(_database, _id),
            new TriggerCollectionNode(_database, _id),
            new IndexCollectionNode(_database, _id)
        });
    }

    public bool Sortable => false;

    public string Query
    {
        get
        {
            var name = new DatabaseObjectMultipartName(null, _database.Name, _schema, _name);
            var connectionString = _database.Databases.Server.ConnectionString;
            string text;
            using (var connection = new SqlConnection(connectionString))
                text = TableNode.GetSelectStatement(connection, name);
            return text;
        }
    }

    public ContextMenu? GetContextMenu()
    {
        var menuItemScriptObject = new MenuItem("Script View as CREATE to clipboard", menuItemScriptObject_Click, EmptyReadOnlyCollection<MenuItem>.Value);
        var items = new[] { menuItemScriptObject }.ToReadOnlyCollection();
        var contextMenu = new ContextMenu(items);
        return contextMenu;
    }

    private void menuItemScriptObject_Click(object sender, EventArgs e)
    {
        var connectionString = _database.Databases.Server.ConnectionString;
        string text;
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            text = SqlDatabase.GetSysComments(connection, _database.Name, _schema, _name, CancellationToken.None).Result;
        }

        var queryForm = (IQueryForm)sender;
        queryForm.SetClipboardText(text);
    }
}