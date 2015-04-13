namespace DataCommander.Providers.OracleBase
{
	using System.Collections.Generic;
	using System.Windows.Forms;

	/// <summary>
    /// Summary description for TablesNode.
    /// </summary>
    public sealed class TriggerNode : ITreeNode
    {
		private TableNode tableNode;
		private string name;

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
    }
}