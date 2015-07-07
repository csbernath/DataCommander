namespace DataCommander.Providers.SqlServer2005
{
    using System.Collections.Generic;
    using System.Windows.Forms;

    internal sealed class FunctionCollectionNode : ITreeNode
    {
        public FunctionCollectionNode(DatabaseNode database)
        {
            this.database = database;
        }

        public string Name
        {
            get
            {
                return "Functions";
            }
        }

        public bool IsLeaf
        {
            get
            {
                return false;
            }
        }

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            return new ITreeNode[]
            {
                new TableValuedFunctionCollectionNode(this.database),
                new ScalarValuedFunctionCollectionNode(this.database)
            };
        }

        public bool Sortable
        {
            get
            {
                return false;
            }
        }

        public string Query
        {
            get
            {
                return null;
            }
        }

        public ContextMenuStrip ContextMenu
        {
            get
            {
                return null;
            }
        }

        readonly DatabaseNode database;
    }
}