using System.Collections.Generic;
using System.Windows.Forms;
using DataCommander.Foundation.Data;

namespace DataCommander.Providers.SQLite.ObjectExplorer
{
    internal sealed class IndexCollectionNode : ITreeNode
    {
        private readonly TableNode _tableNode;

        public IndexCollectionNode(TableNode tableNode)
        {
#if CONTRACTS_FULL
            Contract.Requires(tableNode != null);
#endif

            _tableNode = tableNode;
        }

        #region ITreeNode Members

        string ITreeNode.Name => "Indexes";
        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var commandText = $"PRAGMA index_list({_tableNode.Name});";
            var executor = DbCommandExecutorFactory.Create(_tableNode.Database.Connection);
            var indexNodes = executor.ExecuteReader(new ExecuteReaderRequest(commandText), dataRecord =>
            {
                var name = dataRecord.GetString(0);
                return new IndexNode(_tableNode, name);
            });
            return indexNodes;
        }

        bool ITreeNode.Sortable => false;
        string ITreeNode.Query => null;
        ContextMenuStrip ITreeNode.ContextMenu => null;

        #endregion
    }
}