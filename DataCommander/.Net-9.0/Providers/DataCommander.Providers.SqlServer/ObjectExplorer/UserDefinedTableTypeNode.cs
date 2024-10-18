using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Collections.ReadOnly;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class UserDefinedTableTypeNode(DatabaseNode database, int id, string schema, string name)
    : ITreeNode
{
    string? ITreeNode.Name => $"{schema}.{name}";
    bool ITreeNode.IsLeaf => false;

    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken) => Task.FromResult<IEnumerable<ITreeNode>>(
        [
            new ColumnCollectionNode(database, id)
        ]);

    bool ITreeNode.Sortable => false;
    string ITreeNode.Query => null;

    public ContextMenu? GetContextMenu()
    {
        var menuItems = new MenuItem[]
        {
            new("Script", Script_OnClick, EmptyReadOnlyCollection<MenuItem>.Value)
        }.ToReadOnlyCollection();

        var contextMenu = new ContextMenu(menuItems);
        return contextMenu;
    }

    private void Script_OnClick(object? sender, EventArgs e)
    {
        var queryForm = (IQueryForm)sender!;
        var connectionInfo = SqlObjectScripter.CreateSqlConnectionInfo(database.Databases.Server.ConnectionStringAndCredential);
        var connection = new ServerConnection(connectionInfo);
        connection.Connect();
        var server = new Server(connection);
        var database1 = server.Databases[database.Name];
        var userDefinedTableType = database1.UserDefinedTableTypes[name, schema];
        var stringCollection = userDefinedTableType.Script();

        var sb = new StringBuilder();
        var first = true;
        foreach (var s in stringCollection)
        {
            if (first)
                first = false;
            else
                sb.AppendLine("GO");

            sb.AppendLine(s);
        }

        queryForm.SetClipboardText(sb.ToString());
    }
}