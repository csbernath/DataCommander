using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Data.SqlClient;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class FunctionNode(
    DatabaseNode database,
    string owner,
    string name,
    string xtype)
    : ITreeNode
{
    public string? Name => $"{owner}.{name}";

    public bool IsLeaf => true;

    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken) =>
        Task.FromResult<IEnumerable<ITreeNode>>([]);

    public bool Sortable => false;

    public bool DynamicChildCount => true;

    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken)
    {
        var query = xtype switch
        {
            SqlServerObjectType.ScalarFunction => $"select {database.Name}.{owner}.[{name}]()",
            SqlServerObjectType.TableValuedFunction or SqlServerObjectType.InlineTableValuedFunction => $@"select	*
from	{database.Name}.{owner}.[{name}]()",
            _ => null,
        };
        return Task.FromResult(query);
    }

    public ContextMenu? GetContextMenu()
    {
        var scriptObjectMenuItem = new MenuItem("Script Object", ScriptObjectClicked, []);
        var contextMenu = new ContextMenu([scriptObjectMenuItem]);
        return contextMenu;
    }

    private void ScriptObjectClicked(object? sender, EventArgs e)
    {
        string text;
        using (var connection = database.Databases.Server.CreateConnection())
        {
            connection.Open();
            text = SqlDatabase.GetSysComments(connection, database.Name!, owner, name, CancellationToken.None).Result;
        }

        var queryForm = (IQueryForm)sender!;
        queryForm.ShowText(text);
    }
}