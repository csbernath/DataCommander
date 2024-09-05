using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Microsoft.Data.SqlClient;
using Foundation.Collections.ReadOnly;
using Foundation.Data;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class TriggerNode(DatabaseNode databaseNode, int id, string name) : ITreeNode
{
    public string Name { get; } = name;
    public bool IsLeaf => true;
    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken) => null;
    public bool Sortable => false;
    public string Query => null;

    public ContextMenu? GetContextMenu()
    {
        var menuItemScriptObject = new MenuItem("Script Object", menuItemScriptObject_Click, EmptyReadOnlyCollection<MenuItem>.Value);
        var items = new[] { menuItemScriptObject }.ToReadOnlyCollection();
        var contextMenu = new ContextMenu(items);
        return contextMenu;
    }

    private void menuItemScriptObject_Click(object sender, EventArgs e)
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
            definition = (string)executor.ExecuteScalar(new CreateCommandRequest(commandText));
        }

        var queryForm = (IQueryForm)sender;
        queryForm.ShowText(definition);
    }
}