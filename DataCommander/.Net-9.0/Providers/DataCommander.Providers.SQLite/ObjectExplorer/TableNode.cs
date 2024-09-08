using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Collections.ReadOnly;
using Foundation.Data;

namespace DataCommander.Providers.SQLite.ObjectExplorer;

internal sealed class TableNode : ITreeNode
{
    public TableNode(DatabaseNode databaseNode, string? name)
    {
        DatabaseNode = databaseNode;
        Name = name;
    }

    public DatabaseNode DatabaseNode { get; }

    #region ITreeNode Members

    public string? Name { get; }

    bool ITreeNode.IsLeaf => false;

    public Task<IEnumerable<ITreeNode>> GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        var treeNodes = new ITreeNode[1];
        treeNodes[0] = new IndexCollectionNode(this);
        return Task.FromResult<IEnumerable<ITreeNode>>(treeNodes);
    }
    
    bool ITreeNode.Sortable => false;

    string ITreeNode.Query => $"select\t*\r\nfrom\t{DatabaseNode.Name}.{Name}";

    private static string GetScript(
        SQLiteConnection connection,
        string? databaseName,
        string? name)
    {
        var commandText = $@"
select  sql
from	{databaseName}.sqlite_master
where	name	= '{name}'";
        var executor = DbCommandExecutorFactory.Create(connection);
        var scalar = executor.ExecuteScalar(new CreateCommandRequest(commandText));
        var script = (string) scalar;
        return script;
    }

    private void Script_Click(object sender, EventArgs e)
    {
        string script;
        using (var connection = ConnectionFactory.CreateConnection(DatabaseNode.DatabaseCollectionNode.ConnectionStringAndCredential))
        {
            connection.Open();
            script = GetScript(connection, DatabaseNode.Name, Name);
        }

        var queryForm = (IQueryForm)sender;
        queryForm.ShowText(script);
    }

    public ContextMenu? GetContextMenu()
    {
        ContextMenu contextMenu = null;

        if (Name != "sqlite_master")
        {
            var item = new MenuItem("Script", Script_Click, EmptyReadOnlyCollection<MenuItem>.Value);
            var items = new[] { item }.ToReadOnlyCollection();
            contextMenu = new ContextMenu(items);
        }

        return contextMenu;
    }

    #endregion
}