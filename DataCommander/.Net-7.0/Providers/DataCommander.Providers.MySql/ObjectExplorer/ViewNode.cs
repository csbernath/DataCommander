using System;
using System.Collections.Generic;
using System.Linq;
using DataCommander.Api;
using Foundation.Collections.ReadOnly;
using Foundation.Data;
using MySql.Data.MySqlClient;

namespace DataCommander.Providers.MySql.ObjectExplorer;

internal sealed class ViewNode : ITreeNode
{
    private readonly DatabaseNode _databaseNode;
    private readonly string _name;

    public ViewNode(DatabaseNode databaseNode, string name)
    {
        _databaseNode = databaseNode;
        _name = name;
    }

    string ITreeNode.Name => _name;

    bool ITreeNode.IsLeaf => true;

    IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
    {
        throw new NotImplementedException();
    }

    bool ITreeNode.Sortable => throw new NotImplementedException();

    string ITreeNode.Query => $@"select *
from {_databaseNode.Name}.{_name}";

    public ContextMenu? GetContextMenu()
    {
        var item = new MenuItem("Show create table", ShowCreateTable_Click, EmptyReadOnlyCollection<MenuItem>.Value);
        var items = new[] { item }.ToReadOnlyCollection();
        var menu = new ContextMenu(items);
        return menu;
    }

    private void ShowCreateTable_Click(object sender, EventArgs e)
    {
        var commandText = $"show create table {_databaseNode.Name}.{_name}";
        var createTableStatement = MySqlClientFactory.Instance.ExecuteReader(
            _databaseNode.ObjectExplorer.ConnectionString,
            new ExecuteReaderRequest(commandText),
            128,
            dataRecord => dataRecord.GetString(0)).First();

        var queryForm = (IQueryForm)sender;
        queryForm.ClipboardSetText(createTableStatement);
        queryForm.SetStatusbarPanelText("Copying create table statement to clipboard finished.");
    }
}