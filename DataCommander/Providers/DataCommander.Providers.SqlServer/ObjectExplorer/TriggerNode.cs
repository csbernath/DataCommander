using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Microsoft.Data.SqlClient;
using Foundation.Data;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class TriggerNode(DatabaseNode databaseNode, int id, string? name) : ITreeNode
{
    public string? Name { get; } = name;
    public bool IsLeaf => true;

    public IReadOnlyCollection<string> GetFilterableProperties() => [];

    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(IReadOnlyCollection<FilterCriterion> filterCriteria, bool refresh, CancellationToken cancellationToken) =>
        Task.FromResult<IEnumerable<ITreeNode>>([]);

    public IReadOnlyCollection<FilterCriterion> GetFilterCriteria() => [];

    public bool Sortable => false;

    public bool DynamicChildCount => true;

    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken) => Task.FromResult<string?>(null);

    public ContextMenu? GetContextMenu()
    {
        var menuItemScriptObject = new MenuItem("Script Object", menuItemScriptObject_Click, []);
        var items = new[] { menuItemScriptObject };
        var contextMenu = new ContextMenu(items);
        return contextMenu;
    }

    private void menuItemScriptObject_Click(object? sender, EventArgs e)
    {
        var cb = new SqlCommandBuilder();
        var databaseName = cb.QuoteIdentifier(databaseNode.Name);
        var commandText = $@"select m.definition
from {databaseName}.sys.sql_modules m (nolock)
where m.object_id = {id}";
        string definition;
        using (var connection = databaseNode.Databases.Server.CreateConnection())
        {
            connection.Open();
            var executor = connection.CreateCommandExecutor();
            definition = (string)executor.ExecuteScalar(new CreateCommandRequest(commandText))!;
        }

        var queryForm = (IQueryForm)sender!;
        queryForm.ShowText(definition);
    }
}