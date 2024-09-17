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

    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        return Task.FromResult<IEnumerable<ITreeNode>>(
        [
            new ColumnCollectionNode(database, id),
            new TriggerCollectionNode(database, id),
            new IndexCollectionNode(database, id)
        ]);
    }

    public bool Sortable => false;

    public string Query
    {
        get
        {
            DatabaseObjectMultipartName name1 = new DatabaseObjectMultipartName(null, database.Name, schema, name);
            string text;
            using (Microsoft.Data.SqlClient.SqlConnection connection = database.Databases.Server.CreateConnection())
                text = TableNode.GetSelectStatement(connection, name1);
            return text;
        }
    }

    public ContextMenu? GetContextMenu()
    {
        MenuItem menuItemScriptObject = new MenuItem("Script View as CREATE to clipboard", menuItemScriptObject_Click, EmptyReadOnlyCollection<MenuItem>.Value);
        System.Collections.ObjectModel.ReadOnlyCollection<MenuItem> items = new[] { menuItemScriptObject }.ToReadOnlyCollection();
        ContextMenu contextMenu = new ContextMenu(items);
        return contextMenu;
    }

    private void menuItemScriptObject_Click(object sender, EventArgs e)
    {
        Task<string> task = new Task<string>(() => menuItemScriptObject_ClickAsync(sender).Result);
        task.Start();
        IQueryForm queryForm = (IQueryForm)sender;
        queryForm.SetClipboardText(task.Result);
    }

    private async Task<string> menuItemScriptObject_ClickAsync(object sender)
    {
        string text;
        await using (Microsoft.Data.SqlClient.SqlConnection connection = database.Databases.Server.CreateConnection())
        {
            await connection.OpenAsync();
            text = await SqlDatabase.GetSysComments(connection, database.Name, schema, name, CancellationToken.None);
        }

        return text;
    }
}