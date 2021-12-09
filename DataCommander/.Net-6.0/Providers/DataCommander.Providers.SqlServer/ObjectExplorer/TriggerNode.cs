using System;
using System.Collections.Generic;
using DataCommander.Providers2;
using Microsoft.Data.SqlClient;
using Foundation.Collections.ReadOnly;
using Foundation.Data;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class TriggerNode : ITreeNode
{
    private readonly DatabaseNode _databaseNode;
    private readonly int _id;

    public TriggerNode(DatabaseNode databaseNode, int id, string name)
    {
        _databaseNode = databaseNode;
        _id = id;
        Name = name;
    }

    public string Name { get; }
    public bool IsLeaf => true;
    IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh) => null;
    public bool Sortable => false;
    public string Query => null;

    public ContextMenu GetContextMenu()
    {
        var menuItemScriptObject = new MenuItem("Script Object", menuItemScriptObject_Click, EmptyReadOnlyCollection<MenuItem>.Value);
        var items = new[] { menuItemScriptObject }.ToReadOnlyCollection();
        var contextMenu = new ContextMenu(items);
        return contextMenu;
    }

    private void menuItemScriptObject_Click(object sender, EventArgs e)
    {
        var cb = new SqlCommandBuilder();
        var databaseName = cb.QuoteIdentifier(_databaseNode.Name);

        var commandText = $@"select m.definition
from {databaseName}.sys.sql_modules m (nolock)
where m.object_id = {_id}";

        var connectionString = _databaseNode.Databases.Server.ConnectionString;

        string definition;
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var executor = connection.CreateCommandExecutor();
            definition = (string)executor.ExecuteScalar(new CreateCommandRequest(commandText));
        }

        var queryForm = (IQueryForm)sender;
        queryForm.ShowText(definition);
    }
}