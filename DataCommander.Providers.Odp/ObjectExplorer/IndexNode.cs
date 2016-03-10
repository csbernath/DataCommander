namespace DataCommander.Providers.Odp.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Windows.Forms;

    internal sealed class IndexNode : ITreeNode
    {
        public IndexNode(
          TableNode table,
          string name)
        {
            this.table = table;
            this.name = name;
        }

        public string Name => name;

        public bool IsLeaf => true;

        public IEnumerable<ITreeNode> GetChildren(bool refresh)
        {
            return null;
        }

        public bool Sortable => false;

        public string Query => $@"select column_name from SYS.ALL_IND_COLUMNS
where table_owner = '{table.Schema.Name}' and table_name = '{table.Name
            }' and index_name = '{name}'
order by column_position";

        public ContextMenuStrip ContextMenu => null;

        public void BeforeExpand()
        {
        }

        readonly TableNode table;
        readonly string name;
    }
}