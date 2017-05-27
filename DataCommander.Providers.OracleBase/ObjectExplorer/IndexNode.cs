using System.Collections.Generic;
using System.Windows.Forms;

namespace DataCommander.Providers.OracleBase.ObjectExplorer
{
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
    }
}