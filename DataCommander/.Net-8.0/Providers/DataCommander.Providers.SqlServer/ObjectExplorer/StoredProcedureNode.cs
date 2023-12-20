using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Collections.ReadOnly;
using Foundation.Core;
using Foundation.Data.SqlClient;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class StoredProcedureNode : ITreeNode
{
    private readonly DatabaseNode _database;
    private readonly string _name;
    private readonly string _owner;

    public StoredProcedureNode(DatabaseNode database, string owner, string name)
    {
        _database = database;
        _owner = owner;
        _name = name;
    }

    public string Name => _owner + '.' + _name;
    public bool IsLeaf => true;

    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        return Task.FromResult<IEnumerable<ITreeNode>>(Array.Empty<ITreeNode>());
    }
    
    public bool Sortable => false;

    public string Query
    {
        get
        {
            var query = $"exec {_owner}.{_name}";
            return query;
        }
    }

    public ContextMenu? GetContextMenu()
    {
        var scriptObjectMenuItem = new MenuItem("Script Object", ScriptObjectMenuItem_Click, EmptyReadOnlyCollection<MenuItem>.Value);
        var menuItems = new[] { scriptObjectMenuItem }.ToReadOnlyCollection();
        var contextMenu = new ContextMenu(menuItems);
        return contextMenu;
    }

    private void ScriptObjectMenuItem_Click(object sender, EventArgs e)
    {
        var stopwatch = Stopwatch.StartNew();
        var connectionString = _database.Databases.Server.ConnectionString;
        string text;
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            text = SqlDatabase.GetSysComments(connection, _database.Name, _owner, _name);
        }

        if (text != null)
        {
            var queryForm = (IQueryForm)sender;
            queryForm.SetClipboardText(text);

            queryForm.SetStatusbarPanelText(
                $"Copying stored procedure script to clipboard finished in {StopwatchTimeSpan.ToString(stopwatch.ElapsedTicks, 3)} seconds.");
        }
    }
}