namespace DataCommander.Providers.Odp
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
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

        public string Name
        {
            get
            {
                return "Views";
            }
        }

        public bool IsLeaf
        {
            get
            {
                return false;
            }
        }

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
                return null;
            }
        }

        public ContextMenuStrip ContextMenu
        {
            get
            {
                return null;
            }
        }

        public SchemaNode SchemaNode
        {
            get
            {
                return this.schemaNode;
            }
        }

        public void BeforeExpand()
        {
        }
    }
}