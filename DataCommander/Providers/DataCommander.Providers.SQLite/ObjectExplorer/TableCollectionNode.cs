using System.Collections.Generic;
using System.Windows.Forms;
using Foundation.Data;

namespace DataCommander.Providers.SQLite.ObjectExplorer
{
    internal sealed class TableCollectionNode : ITreeNode
    {
        private readonly DatabaseNode _databaseNode;

        public TableCollectionNode(DatabaseNode databaseNode)
        {
            _databaseNode = databaseNode;
        }

        #region ITreeNode Members

        string ITreeNode.Name => "Tables";

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var commandText = $@"select	name
from
(
	select	name
	from	{_databaseNode.Name}.sqlite_master
	where	type	= 'table'
	union
	select	'sqlite_master'
) t
order by name collate nocase";
            var executor = _databaseNode.Connection.CreateCommandExecutor();
            var table = executor.ExecuteDataTable(new ExecuteReaderRequest(commandText));
            var rows = table.Rows;
            var count = rows.Count;
            var nodes = new ITreeNode[count];

            for (var i = 0; i < count; i++)
            {
                var row = rows[i];
                var name = (string) row["name"];
                nodes[i] = new TableNode(_databaseNode, name);
            }

            return nodes;
        }

        bool ITreeNode.Sortable => false;
        string ITreeNode.Query => null;
        ContextMenuStrip ITreeNode.ContextMenu => null;

        #endregion
    }
}