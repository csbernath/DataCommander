namespace DataCommander.Providers.Odp.ObjectExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Threading;
    using System.Windows.Forms;
    using Foundation.Data;

    internal sealed class ViewCollectionNode : ITreeNode
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
            string commandText = "select view_name from all_views where owner = '{0}' order by view_name";
            commandText = String.Format(commandText, schemaNode.Name);
            var transactionScope = new DbTransactionScope(this.SchemaNode.SchemasNode.Connection, null);
            DataTable dataTable = transactionScope.ExecuteDataTable(new CommandDefinition { CommandText = commandText }, CancellationToken.None);
            DataRowCollection dataRows = dataTable.Rows;
            int count = dataRows.Count;
            var treeNodes = new ITreeNode[count];

            for (int i = 0; i < count; i++)
            {
                string name = (string)dataRows[i][0];
                treeNodes[i] = new ViewNode(this, name);
            }

            return treeNodes;
        }

        public bool Sortable => false;

        public string Query => null;

        public ContextMenuStrip ContextMenu => null;

        public SchemaNode SchemaNode => this.schemaNode;

        public void BeforeExpand()
        {
        }
    }
}