using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Microsoft.Data.SqlClient;
using Foundation.Collections.ReadOnly;
using Foundation.Data;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class TriggerNode(DatabaseNode databaseNode, int id, string? name) : ITreeNode
{
    public string? Name { get; } = name;
    public bool IsLeaf => true;
    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken) => null;
    public bool Sortable => false;
    public string Query => null;

    public ContextMenu? GetContextMenu()
    {
        MenuItem menuItemScriptObject = new MenuItem("Script Object", menuItemScriptObject_Click, EmptyReadOnlyCollection<MenuItem>.Value);
        System.Collections.ObjectModel.ReadOnlyCollection<MenuItem> items = new[] { menuItemScriptObject }.ToReadOnlyCollection();
        ContextMenu contextMenu = new ContextMenu(items);
        return contextMenu;
    }

    private void menuItemScriptObject_Click(object sender, EventArgs e)
    {
        SqlCommandBuilder cb = new SqlCommandBuilder();
        string databaseName = cb.QuoteIdentifier(databaseNode.Name);
        string commandText = $@"select m.definition
from {databaseName}.sys.sql_modules m (nolock)
where m.object_id = {id}";
        string definition;
        using (SqlConnection connection = databaseNode.Databases.Server.CreateConnection())
        {
            connection.Open();
            IDbCommandExecutor executor = connection.CreateCommandExecutor();
            definition = (string)executor.ExecuteScalar(new CreateCommandRequest(commandText));
        }

        IQueryForm queryForm = (IQueryForm)sender;
        queryForm.ShowText(definition);
    }
}