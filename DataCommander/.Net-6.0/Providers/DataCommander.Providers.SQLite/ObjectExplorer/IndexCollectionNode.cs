using System.Collections.Generic;
using Foundation.Assertions;
using Foundation.Data;

namespace DataCommander.Providers.SQLite.ObjectExplorer
{
    internal sealed class IndexCollectionNode : ITreeNode
    {
        private readonly TableNode _tableNode;

        public IndexCollectionNode(TableNode tableNode)
        {
            Assert.IsNotNull(tableNode);
            _tableNode = tableNode;
        }

        string ITreeNode.Name => "Indexes";
        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var commandText = $"PRAGMA index_list({_tableNode.Name});";
            var executor = DbCommandExecutorFactory.Create(_tableNode.Database.Connection);
            return executor.ExecuteReader(new ExecuteReaderRequest(commandText), 128, dataRecord =>
            {
                var name = dataRecord.GetString(0);
                return new IndexNode(_tableNode, name);
            });
        }

        bool ITreeNode.Sortable => false;
        string ITreeNode.Query => null;

        public ContextMenu GetContextMenu()
        {
            throw new System.NotImplementedException();
        }
    }
}