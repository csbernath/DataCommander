using System.Collections.Generic;
using System.Windows.Forms;

namespace DataCommander.Providers.OracleBase.ObjectExplorer
{
    /// <summary>
	/// Summary description for TablesNode.
	/// </summary>
    public sealed class TableNode : ITreeNode
	{
		private readonly SchemaNode schema;
		private readonly string name;
		private readonly bool showFullName;

		public TableNode(
			SchemaNode schema,
			string name,
			bool showFullName )
		{
			this.schema = schema;
			this.name = name;
			this.showFullName = showFullName;
		}

		public string Name => showFullName ? this.schema.Name + "." + this.name : this.name;

        public bool IsLeaf => false;

        public IEnumerable<ITreeNode> GetChildren( bool refresh )
		{
			return new ITreeNode[]
              {
                new TriggerCollectionNode(this),
                new IndexeCollectionNode(this)
              };
		}

		public bool Sortable => false;

        public string Query => "select * from " + schema.Name + "." + name;

        public ContextMenuStrip ContextMenu => null;

        public SchemaNode Schema => schema;
	}
}