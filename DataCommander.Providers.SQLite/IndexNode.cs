namespace DataCommander.Providers.SQLite
{
    using System.Collections.Generic;
    using System.Windows.Forms;
    using DataCommander.Foundation.Data;

    internal sealed class IndexNode : ITreeNode
    {
        private readonly TableNode tableNode;
        private readonly string name;

        public IndexNode( TableNode tableNode, string name )
        {
            this.tableNode = tableNode;
            this.name = name;
        }

        #region ITreeNode Members

        string ITreeNode.Name
        {
            get
            {
                return this.name;
            }
        }

        bool ITreeNode.IsLeaf
        {
            get
            {
                return true;
            }
        }

        IEnumerable<ITreeNode> ITreeNode.GetChildren( bool refresh )
        {
            return null;
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
                string commandText = string.Format(@"select	sql
from	main.sqlite_master
where	type = 'index'
	and name = '{0}'", this.name);
                var transactionScope = new DbTransactionScope(this.tableNode.Database.Connection, null);

                object scalar = transactionScope.ExecuteScalar(new CommandDefinition {CommandText = commandText});
                string sql = Database.GetValueOrDefault<string>(scalar);
                return sql;
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