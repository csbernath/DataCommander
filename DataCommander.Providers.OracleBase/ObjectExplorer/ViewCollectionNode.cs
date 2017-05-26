using Foundation.Data;

namespace DataCommander.Providers.OracleBase
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Windows.Forms;

    public sealed class ViewCollectionNode : ITreeNode
    {
        private readonly SchemaNode schemaNode;

        public ViewCollectionNode(SchemaNode schemaNode)
        {
            this.schemaNode = schemaNode;
        }

        public string Name => "Views";

        public bool IsLeaf => false;

        public IEnumerable<ITreeNode> GetChildren(bool refresh)
        {
            var commandText = "select view_name from all_views where owner = '{0}' order by view_name";
            commandText = string.Format(commandText, schemaNode.Name);
            var transactionScope = new DbTransactionScope(schemaNode.SchemasNode.Connection, null);
            var dataTable = transactionScope.ExecuteDataTable(new CommandDefinition {CommandText = commandText}, CancellationToken.None);
            var dataRows = dataTable.Rows;
            var count = dataRows.Count;
            var treeNodes = new ITreeNode[count];

            for (var i = 0; i < count; i++)
            {
                var name = (string) dataRows[i][0];
                treeNodes[i] = new ViewNode(this, name);
            }

            return treeNodes;
        }

        public bool Sortable => false;

        public string Query => null;

        public ContextMenuStrip ContextMenu => null;

        public SchemaNode SchemaNode => this.schemaNode;
    }
}