using System.Collections.Generic;
using System.Windows.Forms;

namespace DataCommander.Providers.Odp.ObjectExplorer
{
    //using Core = Citibank.Infociti.Core.Data;

	/// <summary>
	/// Summary description for TablesNode.
	/// </summary>
	internal sealed class TableNode : ITreeNode
	{
		private readonly SchemaNode _schema;
		private readonly string _name;
		private readonly bool _showFullName;

		public TableNode(
			SchemaNode schema,
			string name,
			bool showFullName )
		{
			_schema = schema;
			_name = name;
			_showFullName = showFullName;
		}

		public string Name => _showFullName ? _schema.Name + "." + _name : _name;

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
	    public string Query => "select * from " + _schema.Name + "." + _name;
	    public ContextMenuStrip ContextMenu => null;


		public SchemaNode Schema => _schema;
	}
}