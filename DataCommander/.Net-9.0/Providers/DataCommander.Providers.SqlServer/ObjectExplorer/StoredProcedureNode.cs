using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Collections.ReadOnly;
using Foundation.Core;
using Foundation.Data.SqlClient;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class StoredProcedureNode(DatabaseNode database, string owner, string name) : ITreeNode
{
    public string? Name => owner + '.' + name;
    public bool IsLeaf => true;

    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken) => Task.FromResult<IEnumerable<ITreeNode>>([]);

    public bool Sortable => false;

    public string Query
    {
        get
        {
            var query = $"exec {owner}.{name}";
            return query;
        }
    }

    public ContextMenu? GetContextMenu()
    {
        var scriptObjectMenuItem = new MenuItem("Script Object", ScriptObjectMenuItem_Click, EmptyReadOnlyCollection<MenuItem>.Value);
        System.Collections.ObjectModel.ReadOnlyCollection<MenuItem> menuItems = new[] { scriptObjectMenuItem }.ToReadOnlyCollection();
        var contextMenu = new ContextMenu(menuItems);
        return contextMenu;
    }

    private void ScriptObjectMenuItem_Click(object sender, EventArgs e)
    {
        var stopwatch = Stopwatch.StartNew();
        var queryForm = (IQueryForm)sender;
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        var cancelableOperationForm = queryForm.CreateCancelableOperationForm(cancellationTokenSource, TimeSpan.FromSeconds(1),
            "Getting stored procedure text...", "Please wait...");
        var text = cancelableOperationForm.Execute(new Task<string?>(() => GetText(cancellationToken).Result));
        if (!string.IsNullOrEmpty(text))
        {
            queryForm.SetClipboardText(text);
            queryForm.SetStatusbarPanelText(
                $"Copying stored procedure script to clipboard finished in {StopwatchTimeSpan.ToString(stopwatch.ElapsedTicks, 3)} seconds.");
        }
    }

    private async Task<string?> GetText(CancellationToken cancellationToken)
    {
        await using var connection = database.Databases.Server.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        var text = await SqlDatabase.GetSysComments(connection, database.Name, owner, name, cancellationToken);
        return text;
    }
}