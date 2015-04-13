namespace DataCommander.Providers.Odp
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Windows.Forms;
	using DataCommander.Foundation.Data;

    internal sealed class ViewCollectionNode : ITreeNode
    {
		private SchemaNode schemaNode;

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
            DataTable dataTable = schemaNode.SchemasNode.Connection.ExecuteDataTable(commandText);
            DataRowCollection dataRows = dataTable.Rows;
            int count = dataRows.Count;
            ITreeNode[] treeNodes = new ITreeNode[count];

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