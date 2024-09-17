using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Collections.ReadOnly;
using Foundation.Data;

namespace DataCommander.Providers.SQLite.ObjectExplorer;

internal sealed class TableNode(DatabaseNode databaseNode, string? name) : ITreeNode
{
    public DatabaseNode DatabaseNode { get; } = databaseNode;

    #region ITreeNode Members

    public string? Name { get; } = name;

    bool ITreeNode.IsLeaf => false;

    public Task<IEnumerable<ITreeNode>> GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        ITreeNode[] treeNodes = new ITreeNode[1];
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
        string commandText = $@"
select  sql
from	{databaseName}.sqlite_master
where	name	= '{name}'";
        IDbCommandAsyncExecutor executor = DbCommandExecutorFactory.Create(connection);
        object scalar = executor.ExecuteScalar(new CreateCommandRequest(commandText));
        string script = (string) scalar;
        return script;
    }

    private void Script_Click(object sender, EventArgs e)
    {
        string script;
        using (SQLiteConnection connection = ConnectionFactory.CreateConnection(DatabaseNode.DatabaseCollectionNode.ConnectionStringAndCredential))
        {
            connection.Open();
            script = GetScript(connection, DatabaseNode.Name, Name);
        }

        IQueryForm queryForm = (IQueryForm)sender;
        queryForm.ShowText(script);
    }

    public ContextMenu? GetContextMenu()
    {
        ContextMenu contextMenu = null;

        if (Name != "sqlite_master")
        {
            MenuItem item = new MenuItem("Script", Script_Click, EmptyReadOnlyCollection<MenuItem>.Value);
            System.Collections.ObjectModel.ReadOnlyCollection<MenuItem> items = new[] { item }.ToReadOnlyCollection();
            contextMenu = new ContextMenu(items);
        }

        return contextMenu;
    }

    #endregion
}