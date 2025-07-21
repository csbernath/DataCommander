using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Collections.ReadOnly;
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
        Task.FromResult<IEnumerable<ITreeNode>>(Array.Empty<ITreeNode>());

    public bool Sortable => false;

    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken)
    {
        //string query = string.Format("select {0}.{1}.[{2}]()",database.Name,owner,name);
        var query = xtype switch
        {
            //Scalar function
            "FN" => $"select {database.Name}.{owner}.[{name}]()",
            //Table function
            "TF" or "IF" => $@"select	*
from	{database.Name}.{owner}.[{name}]()",
            _ => null,
        };
        return Task.FromResult(query);
    }

    public ContextMenu? GetContextMenu()
    {
        var scriptObjectMenuItem = new MenuItem("Script Object", menuItemScriptObject_Click, EmptyReadOnlyCollection<MenuItem>.Value);
        var menuItems = new[] { scriptObjectMenuItem }.ToReadOnlyCollection();
        var contextMenu = new ContextMenu(menuItems);
        return contextMenu;
    }

    private void menuItemScriptObject_Click(object? sender, EventArgs e)
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