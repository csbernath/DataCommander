using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Microsoft.Data.SqlClient;
using Foundation.Collections.ReadOnly;
using Foundation.Data.SqlClient;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class ViewNode(DatabaseNode database, int id, string? schema, string? name)
    : ITreeNode
{
    public string Name => $"{schema}.{name}";
    public bool IsLeaf => false;

    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        return Task.FromResult<IEnumerable<ITreeNode>>(new ITreeNode[]
        {
            new ColumnCollectionNode(database, id),
            new TriggerCollectionNode(database, id),
            new IndexCollectionNode(database, id)
        });
    }

    public bool Sortable => false;

    public string Query
    {
        get
        {
            var name1 = new DatabaseObjectMultipartName(null, database.Name, schema, name);
            var connectionString = database.Databases.Server.ConnectionString;
            string text;
            using (var connection = new SqlConnection(connectionString))
                text = TableNode.GetSelectStatement(connection, name1);
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
        var connectionString = database.Databases.Server.ConnectionString;
        string text;
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            text = SqlDatabase.GetSysComments(connection, database.Name, schema, name, CancellationToken.None).Result;
        }

        var queryForm = (IQueryForm)sender;
        queryForm.SetClipboardText(text);
    }
}