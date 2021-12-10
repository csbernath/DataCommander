using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DataCommander.Providers2;
using Foundation.Collections.ReadOnly;
using Foundation.Data;
using MySql.Data.MySqlClient;

namespace DataCommander.Providers.MySql.ObjectExplorer;

internal sealed class StoredProcedureNode : ITreeNode
{
    private readonly DatabaseNode _databaseNode;
    private readonly string _name;

    public StoredProcedureNode(DatabaseNode databaseNode, string name)
    {
        _databaseNode = databaseNode;
        _name = name;
    }

    string ITreeNode.Name => _name;
    bool ITreeNode.IsLeaf => true;
    IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh) => null;
    bool ITreeNode.Sortable => false;
    string ITreeNode.Query => null;

    public ContextMenu GetContextMenu()
    {
        var item = new MenuItem("Show create procedure", ShowCreateProcedure_Click, EmptyReadOnlyCollection<MenuItem>.Value);
        var items = new[] { item }.ToReadOnlyCollection();
        var menu = new ContextMenu(items);
        return menu;
    }

    private void ShowCreateProcedure_Click(object sender, EventArgs e)
    {
        var commandText = $"show create procedure {_databaseNode.Name}.{_name}";
        var statement = MySqlClientFactory.Instance.ExecuteReader(
            _databaseNode.ObjectExplorer.ConnectionString,
            new ExecuteReaderRequest(commandText),
            128,
            dataRecord => dataRecord.GetString(2)).First();

        var queryForm = (IQueryForm)sender;
        queryForm.ClipboardSetText(statement);        
        queryForm.SetStatusbarPanelText("Copying create procedure statement to clipboard finished.", SystemColors.ControlText);
    }
}