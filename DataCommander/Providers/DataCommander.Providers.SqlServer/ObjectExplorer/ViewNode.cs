using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Collections.ReadOnly;
using Foundation.Data.SqlClient;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class ViewNode(DatabaseNode database, int id, string? schema, string? name)
    : ITreeNode
{
    public string? Name => $"{schema}.{name}";
    public bool IsLeaf => false;

    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken) => Task.FromResult<IEnumerable<ITreeNode>>(
        [
            new ColumnCollectionNode(database, id),
            new TriggerCollectionNode(database, id),
            new IndexCollectionNode(database, id)
        ]);

    public bool Sortable => false;

    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken)
    {
        var name1 = new DatabaseObjectMultipartName(null, database.Name, schema, name);
        string text;
        using (var connection = database.Databases.Server.CreateConnection())
            text = TableNode.GetSelectStatement(connection, name1);
        return Task.FromResult(text);
    }

    public ContextMenu? GetContextMenu()
    {
        var menuItemScriptObject = new MenuItem("Script View as CREATE to clipboard", menuItemScriptObject_Click, EmptyReadOnlyCollection<MenuItem>.Value);
        var items = new[] { menuItemScriptObject }.ToReadOnlyCollection();
        var contextMenu = new ContextMenu(items);
        return contextMenu;
    }

    private void menuItemScriptObject_Click(object? sender, EventArgs e)
    {
        var task = new Task<string>(() => menuItemScriptObject_ClickAsync(sender).Result);
        task.Start();
        var queryForm = (IQueryForm)sender!;
        queryForm.SetClipboardText(task.Result);
    }

    private async Task<string> menuItemScriptObject_ClickAsync(object? sender)
    {
        await using var connection = database.Databases.Server.CreateConnection();
        await connection.OpenAsync();
        var text = await SqlDatabase.GetSysComments(connection, database.Name!, schema!, name!, CancellationToken.None);
        return text;
    }
}