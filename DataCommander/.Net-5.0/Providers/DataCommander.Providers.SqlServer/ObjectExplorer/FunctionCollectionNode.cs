using System.Collections.Generic;
using System.Windows.Forms;

namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    internal sealed class FunctionCollectionNode : ITreeNode
    {
        private readonly DatabaseNode _database;

        public FunctionCollectionNode(DatabaseNode database) => _database = database;

        public string Name => "Functions";
        public bool IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh) =>
            new ITreeNode[]
            {
                new TableValuedFunctionCollectionNode(_database),
                new ScalarValuedFunctionCollectionNode(_database)
            };

        public bool Sortable => false;
        public string Query => null;
        public ContextMenuStrip ContextMenu => null;
        public ContextMenu GetContextMenu()
        {
            throw new System.NotImplementedException();
        }
    }
}