namespace DataCommander.Providers.OracleBase
{
    using System.Collections.Generic;
    using System.Windows.Forms;

    /// <summary>
    /// Summary description for TablesNode.
    /// </summary>
    public sealed class TriggerNode : ITreeNode
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

        public string Name => name;

        public bool IsLeaf => true;

        public IEnumerable<ITreeNode> GetChildren(bool refresh)
        {
            return null;
        }

        public bool Sortable => false;

        public string Query => "select * from " + tableNode.Schema.Name + "." + name;

        public ContextMenuStrip ContextMenu => null;
    }
}