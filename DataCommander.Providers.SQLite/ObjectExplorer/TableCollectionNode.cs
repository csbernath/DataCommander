namespace DataCommander.Providers.SQLite
{
    using System.Collections.Generic;
    using System.Data;
    using System.Threading;
    using System.Windows.Forms;
    using DataCommander.Foundation.Data;

    internal sealed class TableCollectionNode : ITreeNode
    {
        private readonly DatabaseNode databaseNode;

        public TableCollectionNode(DatabaseNode databaseNode)
        {
            this.databaseNode = databaseNode;
        }

        #region ITreeNode Members

        string ITreeNode.Name
        {
            get
            {
                return "Tables";
            }
        }

        bool ITreeNode.IsLeaf
        {
            get
            {
                return false;
            }
        }

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            string commandText =
                $@"
select	name
from
(
	select	name
	from	{this.databaseNode.Name
                    }.sqlite_master
	where	type	= 'table'
	union
	select	'sqlite_master'
) t
order by name collate nocase";
            Database database = new Database(this.databaseNode.Connection);
            DataTable table = database.ExecuteDataTable(commandText, CancellationToken.None);
            DataRowCollection rows = table.Rows;
            int count = rows.Count;
            ITreeNode[] nodes = new ITreeNode[count];

            for (int i=0;i<count;i++)
            {
                DataRow row = rows[i];
                string name = (string)row["name"];
                nodes[i] = new TableNode(this.databaseNode, name);
            }

            return nodes;
        }

        bool ITreeNode.Sortable
        {
            get
            {
                return false;
            }
        }

        string ITreeNode.Query
        {
            get
            {
                return null;
            }
        }

        ContextMenuStrip ITreeNode.ContextMenu
        {
            get
            {
                return null;
            }
        }

        #endregion
    }
}