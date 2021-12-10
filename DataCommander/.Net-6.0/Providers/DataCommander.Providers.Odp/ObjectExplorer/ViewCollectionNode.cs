using System.Collections.Generic;
using DataCommander.Providers2;
using Foundation.Data;

namespace DataCommander.Providers.Odp.ObjectExplorer;

internal sealed class ViewCollectionNode : ITreeNode
{
    private readonly SchemaNode _schemaNode;

    public ViewCollectionNode(SchemaNode schemaNode)
    {
        _schemaNode = schemaNode;
    }

    public string Name => "Views";

    public bool IsLeaf => false;

    public IEnumerable<ITreeNode> GetChildren(bool refresh)
    {
        var commandText = "select view_name from all_views where owner = '{0}' order by view_name";
        commandText = string.Format(commandText, _schemaNode.Name);
        var executor = SchemaNode.SchemasNode.Connection.CreateCommandExecutor();
        var dataTable = executor.ExecuteDataTable(new ExecuteReaderRequest(commandText));
        var dataRows = dataTable.Rows;
        var count = dataRows.Count;
        var treeNodes = new ITreeNode[count];

        for (var i = 0; i < count; i++)
        {
            var name = (string) dataRows[i][0];
            treeNodes[i] = new ViewNode(this, name);
        }

        return treeNodes;
    }

    public bool Sortable => false;
    public string Query => null;

    public ContextMenu GetContextMenu()
    {
        throw new System.NotImplementedException();
    }

    public SchemaNode SchemaNode => _schemaNode;
}