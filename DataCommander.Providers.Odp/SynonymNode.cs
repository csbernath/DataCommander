namespace DataCommander.Providers.Odp
{
    using System.Collections.Generic;
    using System.Data;
    using System.Windows.Forms;
    using Foundation.Data;

    internal sealed class SynonymNode : ITreeNode
	{
		private readonly SchemaNode schema;
		private readonly string name;

		public SynonymNode( SchemaNode schema, string name )
		{
			this.schema = schema;
			this.name = name;
		}

		#region ITreeNode Members

		string ITreeNode.Name
		{
			get
			{
				return this.name;
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
            string commandText = string.Format(@"
select	s.TABLE_OWNER,
	s.TABLE_NAME
from	SYS.ALL_SYNONYMS s
where	s.OWNER			= '{0}'
	and s.SYNONYM_NAME	= '{1}'", this.schema.Name, this.name);
            var transactionScope = new DbTransactionScope(this.schema.SchemasNode.Connection, null);
            DataTable dataTable = transactionScope.ExecuteDataTable(new CommandDefinition {CommandText = commandText});
            DataRow dataRow = dataTable.Rows[0];
            string schemaName = (string)dataRow["TABLE_OWNER"];
            var schemaNode = new SchemaNode(this.schema.SchemasNode, schemaName);
            var tableNode = new TableNode(schemaNode, (string)dataRow["TABLE_NAME"], true);
            return new ITreeNode[] {tableNode};
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

		#endregion
	}
}
