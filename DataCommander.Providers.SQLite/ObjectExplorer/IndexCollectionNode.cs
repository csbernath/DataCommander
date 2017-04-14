namespace DataCommander.Providers.SQLite
{
    using System.Collections.Generic;
    using System.Data;
    using System.Windows.Forms;
    using DataCommander.Foundation.Data;

    internal sealed class IndexCollectionNode : ITreeNode
    {
        private readonly TableNode tableNode;

        public IndexCollectionNode(TableNode tableNode)
        {
#if CONTRACTS_FULL
            Contract.Requires(tableNode != null);
#endif

            this.tableNode = tableNode;
        }

#region ITreeNode Members

        string ITreeNode.Name => "Indexes";

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            //            string commandText = string.Format(@"select	idx
            //from	main.sqlite_stat1
            //where	tbl = '{0}'
            //order by idx", this.tableNode.Name);
            var commandText = $"PRAGMA index_list({this.tableNode.Name});";
            var children = new List<ITreeNode>();
            var transactionScope = new DbTransactionScope(this.tableNode.Database.Connection, null);

            using (var dataReader = transactionScope.ExecuteReader(new CommandDefinition {CommandText = commandText}, CommandBehavior.Default))
            {
                dataReader.Read(dataRecord =>
                {
                    var name = dataRecord.GetString(0);
                    var indexNode = new IndexNode(this.tableNode, name);
                    children.Add(indexNode);
                });
            }

            return children;
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => null;

        ContextMenuStrip ITreeNode.ContextMenu => null;

#endregion
    }
}