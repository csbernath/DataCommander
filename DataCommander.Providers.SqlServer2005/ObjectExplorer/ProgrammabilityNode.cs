namespace DataCommander.Providers.SqlServer2005
{
    using System.Collections.Generic;
    using System.Windows.Forms;

    internal sealed class ProgrammabilityNode : ITreeNode
    {
        private readonly DatabaseNode database;

        public ProgrammabilityNode(DatabaseNode database)
        {
            this.database = database;
        }

        string ITreeNode.Name
        {
            get
            {
                return "Programmability";
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
            return new ITreeNode[]
            {
                new StoredProcedureCollectionNode(this.database, false),
                new FunctionCollectionNode(this.database),
                new UserDefinedTableTypeCollectionNode(this.database)
            };
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
    }
}