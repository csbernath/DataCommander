namespace SqlUtil.Providers.Odp
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Windows.Forms;
    using Oracle.DataAccess.Client;
    using Binarit.Foundation.Data;

    internal class IndexesNode : ITreeNode
    {
        public IndexesNode(TableNode tableNode)
        {
            this.table = tableNode;
        }

        public string Name
        {
            get
            {
                return "Indexes";
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
            string commandText = "select index_name from sys.all_indexes where owner = '{0}' and table_name = '{1}' order by index_name";
            commandText = string.Format(commandText, table.Schema.Name, table.Name);            
            OracleCommand command = new OracleCommand(commandText, table.Schema.SchemasNode.Connection);
            command.FetchSize = 256 * 1024;
            DataTable dataTable = command.ExecuteDataTable();
            int count = dataTable.Rows.Count;
            string[] indexes = new string[count];

            for (int i = 0; i < count; i++)
            {
                string name = (string)dataTable.Rows[i][0];
                indexes[i] = name;
            }

            ITreeNode[] treeNodes = new ITreeNode[indexes.Length];

            for (int i = 0; i < indexes.Length; i++)
                treeNodes[i] = new IndexNode(table, indexes[i]);

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

        public void BeforeExpand()
        {
        }

        TableNode table;
    }
}