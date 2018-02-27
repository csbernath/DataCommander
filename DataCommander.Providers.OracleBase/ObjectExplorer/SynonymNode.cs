using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using Foundation.Data;

namespace DataCommander.Providers.OracleBase.ObjectExplorer
{
    public sealed class SynonymNode : ITreeNode
	{
		private readonly SchemaNode schema;
		private readonly string name;

		public SynonymNode( SchemaNode schema, string name )
		{
			this.schema = schema;
			this.name = name;
		}

		#region ITreeNode Members

		string ITreeNode.Name => name;

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var commandText =
                $@"
select	s.TABLE_OWNER,
	s.TABLE_NAME
from	SYS.ALL_SYNONYMS s
where	s.OWNER			= '{schema.Name}'
	and s.SYNONYM_NAME	= '{name}'";
            var transactionScope = new DbTransactionScope(schema.SchemasNode.Connection, null);
            var dataTable = transactionScope.ExecuteDataTable(new CommandDefinition {CommandText = commandText}, CancellationToken.None);
            var dataRow = dataTable.Rows[0];
            var schemaName = (string)dataRow["TABLE_OWNER"];
            var schemaNode = new SchemaNode(schema.SchemasNode, schemaName);
            var tableNode = new TableNode(schemaNode, (string)dataRow["TABLE_NAME"], true);
            return new ITreeNode[] {tableNode};
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => null;

        ContextMenuStrip ITreeNode.ContextMenu => null;

        #endregion
	}
}
