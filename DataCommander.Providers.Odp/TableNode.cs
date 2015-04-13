namespace DataCommander.Providers.Odp
{
    using System.Collections.Generic;
    using System.Windows.Forms;
    //using Core = Citibank.Infociti.Core.Data;

	/// <summary>
	/// Summary description for TablesNode.
	/// </summary>
	internal sealed class TableNode : ITreeNode
	{
		private SchemaNode schema;
		private string name;
		private bool showFullName;

		public TableNode(
			SchemaNode schema,
			string name,
			bool showFullName )
		{
			this.schema = schema;
			this.name = name;
			this.showFullName = showFullName;
		}

		public string Name
		{
			get
			{
				return showFullName ? this.schema.Name + "." + this.name : this.name;
			}
		}

		public bool IsLeaf
		{
			get
			{
				return false;
			}
		}

		public IEnumerable<ITreeNode> GetChildren( bool refresh )
		{
			return new ITreeNode[]
              {
                new TriggerCollectionNode(this),
                new IndexeCollectionNode(this)
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
				return "select * from " + schema.Name + "." + name;
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

		public SchemaNode Schema
		{
			get
			{
				return schema;
			}
		}
	}
}