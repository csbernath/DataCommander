namespace DataCommander.Providers.SQLite
{
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.Contracts;
    using System.Windows.Forms;
    using DataCommander.Foundation.Data;

    internal sealed class IndexCollectionNode : ITreeNode
    {
        private readonly TableNode tableNode;

        public IndexCollectionNode(TableNode tableNode)
        {
            Contract.Requires(tableNode != null);

            this.tableNode = tableNode;
        }

        #region ITreeNode Members

        string ITreeNode.Name
        {
            get
            {
                return "Indexes";
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
            //            string commandText = string.Format(@"select	idx
            //from	main.sqlite_stat1
            //where	tbl = '{0}'
            //order by idx", this.tableNode.Name);
            string commandText = string.Format("PRAGMA index_list({0});", this.tableNode.Name);
            var children = new List<ITreeNode>();
            var transactionScope = new DbTransactionScope(this.tableNode.Database.Connection, null);

            using (var dataReader = transactionScope.ExecuteReader(new CommandDefinition {CommandText = commandText}, CommandBehavior.Default))
            {
                dataReader.Read(dataRecord =>
                {
                    string name = dataRecord.GetString(0);
                    IndexNode indexNode = new IndexNode(this.tableNode, name);
                    children.Add(indexNode);
                });
            }

            return children;
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