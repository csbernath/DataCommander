namespace DataCommander.Providers.Odp
{
    using System.Collections.Generic;
    using System.Windows.Forms;

    /// <summary>
    /// Summary description for TablesNode.
    /// </summary>
    internal sealed class TriggerNode : ITreeNode
    {
		private readonly TableNode tableNode;
		private readonly string name;

        public TriggerNode(
          TableNode tableNode,
          string name)
        {
            this.tableNode = tableNode;
            this.name = name;
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public bool IsLeaf
        {
            get
            {
                return true;
            }
        }

        public IEnumerable<ITreeNode> GetChildren(bool refresh)
        {
            return null;
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
                return "select * from " + tableNode.Schema.Name + "." + name;
            }
        }

        public ContextMenuStrip ContextMenu
        {
            get
            {
                return null;
            }
        }

        public void BeforeExpand()
        {
        }
    }
}