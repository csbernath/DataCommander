using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using DataCommander.Foundation.Data;

namespace DataCommander.Providers.SQLite.ObjectExplorer
{
    internal sealed class TableCollectionNode : ITreeNode
    {
        private readonly DatabaseNode databaseNode;

        public TableCollectionNode(DatabaseNode databaseNode)
        {
            this.databaseNode = databaseNode;
        }

        #region ITreeNode Members

        string ITreeNode.Name => "Tables";

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var commandText =
                $@"
select	name
from
(
	select	name
	from	{this.databaseNode.Name}.sqlite_master
	where	type	= 'table'
	union
	select	'sqlite_master'
) t
order by name collate nocase";
            var database = new Database(this.databaseNode.Connection);
            var table = database.ExecuteDataTable(commandText, CancellationToken.None);
            var rows = table.Rows;
            var count = rows.Count;
            var nodes = new ITreeNode[count];

            for (var i=0;i<count;i++)
            {
                var row = rows[i];
                var name = (string)row["name"];
                nodes[i] = new TableNode(this.databaseNode, name);
            }

            return nodes;
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => null;

        ContextMenuStrip ITreeNode.ContextMenu => null;

        #endregion
    }
}