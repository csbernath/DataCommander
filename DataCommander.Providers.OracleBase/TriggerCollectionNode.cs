namespace DataCommander.Providers.OracleBase
{
    using System.Collections.Generic;
    using System.Data;
    using System.Windows.Forms;
    using DataCommander.Foundation.Data;

    /// <summary>
    /// Summary description for TablesNode.
    /// </summary>
    public sealed class TriggerCollectionNode : ITreeNode
    {
        public TriggerCollectionNode(TableNode tableNode)
        {
            this.table = tableNode;
        }

        public string Name
        {
            get
            {
                return "Triggers";
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
            string commandText = "select trigger_name from sys.dba_triggers where table_owner = '{0}' and table_name = '{1}' order by trigger_name";
            commandText = string.Format(commandText, table.Schema.Name, table.Name);

            DataTable dataTable = new DataTable();
            IDbCommand command = table.Schema.SchemasNode.Connection.CreateCommand();
            // TODO
            // command.FetchSize = 256 * 1024;
            command.Fill(dataTable);
            int count = dataTable.Rows.Count;
            string[] triggers = new string[count];

            for (int i = 0; i < count; i++)
            {
                string name = (string)dataTable.Rows[i][0];
                triggers[i] = name;
            }

            ITreeNode[] treeNodes = new ITreeNode[triggers.Length];

            for (int i = 0; i < triggers.Length; i++)
                treeNodes[i] = new TriggerNode(table, triggers[i]);

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

        TableNode table;
    }
}