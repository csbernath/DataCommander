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

    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        return Task.FromResult<IEnumerable<ITreeNode>>(
        [
            new ColumnCollectionNode(database, id)
        ]);
    }

    bool ITreeNode.Sortable => false;
    string ITreeNode.Query => null;

    public ContextMenu? GetContextMenu()
    {
        System.Collections.ObjectModel.ReadOnlyCollection<MenuItem> menuItems = new MenuItem[]
        {
            new("Script", Script_OnClick, EmptyReadOnlyCollection<MenuItem>.Value)
        }.ToReadOnlyCollection();

        ContextMenu contextMenu = new ContextMenu(menuItems);
        return contextMenu;
    }

    private void Script_OnClick(object? sender, EventArgs e)
    {
        IQueryForm queryForm = (IQueryForm)sender!;
        SqlConnectionInfo connectionInfo = SqlObjectScripter.CreateSqlConnectionInfo(database.Databases.Server.ConnectionStringAndCredential);
        ServerConnection connection = new ServerConnection(connectionInfo);
        connection.Connect();
        Server server = new Server(connection);
        Database database1 = server.Databases[database.Name];
        UserDefinedTableType userDefinedTableType = database1.UserDefinedTableTypes[name, schema];
        System.Collections.Specialized.StringCollection stringCollection = userDefinedTableType.Script();

        StringBuilder sb = new StringBuilder();
        bool first = true;
        foreach (string? s in stringCollection)
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