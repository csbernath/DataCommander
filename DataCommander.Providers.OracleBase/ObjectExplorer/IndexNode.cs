namespace DataCommander.Providers.OracleBase
{
    using System.Collections.Generic;
    using System.Windows.Forms;

    internal sealed class IndexNode : ITreeNode
    {
        private readonly TableNode table;
        private readonly string name;

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
                return
                    $@"select column_name from SYS.ALL_IND_COLUMNS
where table_owner = '{table.Schema.Name}' and table_name = '{table.Name
                        }' and index_name = '{name}'
order by column_position";
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