using System.Collections.Generic;
using System.Windows.Forms;
using Foundation.Data;

namespace DataCommander.Providers.SQLite.ObjectExplorer
{
    internal sealed class IndexNode : ITreeNode
    {
        private readonly TableNode tableNode;
        private readonly string name;

        public IndexNode(TableNode tableNode, string name)
        {
            this.tableNode = tableNode;
            this.name = name;
        }

        #region ITreeNode Members

        string ITreeNode.Name => this.name;

        bool ITreeNode.IsLeaf => true;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            return null;
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query
        {
            get
            {
                var commandText = $@"select sql
from main.sqlite_master
where
    type = 'index'
    and name = '{this.name}'";
                var executor = DbCommandExecutorFactory.Create(this.tableNode.Database.Connection);
                var sql = (string) executor.ExecuteScalar(new CreateCommandRequest(commandText));
                return sql;
            }
        }

        ContextMenuStrip ITreeNode.ContextMenu => null;

        #endregion
    }
}