using System.Collections.Generic;
using System.Data;
using System.Threading;
using DataCommander.Api;
using Foundation.Data;
using Oracle.ManagedDataAccess.Client;

namespace DataCommander.Providers.Odp.ObjectExplorer;

/// <summary>
/// Summary description for TablesNode.
/// </summary>
internal sealed class TriggerCollectionNode : ITreeNode
{
    public TriggerCollectionNode(TableNode tableNode)
    {
        _table = tableNode;
    }

    public string Name => "Triggers";

    public bool IsLeaf => false;

    public IEnumerable<ITreeNode> GetChildren(bool refresh)
    {
        var commandText = "select trigger_name from sys.dba_triggers where table_owner = '{0}' and table_name = '{1}' order by trigger_name";
        commandText = string.Format(commandText, _table.Schema.Name, _table.Name);

        var dataTable = new DataTable();
        var command = new OracleCommand(commandText, _table.Schema.SchemasNode.Connection);
        command.FetchSize = 256 * 1024;
        command.Fill(dataTable, CancellationToken.None);
        var count = dataTable.Rows.Count;
        var triggers = new string[count];

        for (var i = 0; i < count; i++)
        {
            var name = (string)dataTable.Rows[i][0];
            triggers[i] = name;
        }

        var treeNodes = new ITreeNode[triggers.Length];

        for (var i = 0; i < triggers.Length; i++)
            treeNodes[i] = new TriggerNode(_table, triggers[i]);

        return treeNodes;
    }

    public bool Sortable => false;
    public string Query => null;

    public ContextMenu GetContextMenu()
    {
        throw new System.NotImplementedException();
    }

    private readonly TableNode _table;
}