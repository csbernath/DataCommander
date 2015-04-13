namespace DataCommander.Providers.OracleBase
{
    using System.Collections.Generic;
    using System.Windows.Forms;

    internal sealed class IndexNode : ITreeNode
    {
        private TableNode table;
        private string name;

        public IndexNode(
          TableNode table,
          string name)
        {
            this.table = table;
            this.name = name;
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public bool IsLeaf
        {
            get
            {
                return true;
            }
        }

        public IEnumerable<ITreeNode> GetChildren(bool refresh)
        {
            return null;
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
                return string.Format(@"select column_name from SYS.ALL_IND_COLUMNS
where table_owner = '{0}' and table_name = '{1}' and index_name = '{2}'
order by column_position", table.Schema.Name, table.Name, name);
            }
        }

        public ContextMenuStrip ContextMenu
        {
            get
            {
                return null;
            }
        }
    }
}