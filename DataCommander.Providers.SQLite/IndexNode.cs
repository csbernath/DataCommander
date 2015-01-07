namespace DataCommander.Providers.SQLite
{
    using System.Collections.Generic;
    using System.Data;
    using DataCommander.Foundation.Data;

    internal sealed class IndexNode : ITreeNode
    {
        private TableNode tableNode;
        private string name;

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
                string commandText = string.Format( @"select	sql
from	main.sqlite_master
where	type = 'index'
	and name = '{0}'", this.name );

                object scalar = this.tableNode.Database.Connection.ExecuteScalar( null, commandText, CommandType.Text, 0 );
                string sql = DataCommander.Foundation.Data.Database.GetValueOrDefault<string>( scalar );
                return sql;
            }
        }

        System.Windows.Forms.ContextMenuStrip ITreeNode.ContextMenu
        {
            get
            {
                return null;
            }
        }

        #endregion
    }
}