using System.Collections.Generic;
using System.Windows.Forms;

namespace DataCommander.Providers.Odp.ObjectExplorer
{
    internal sealed class IndexNode : ITreeNode
    {
        public IndexNode(
          TableNode table,
          string name)
        {
            _table = table;
            _name = name;
        }

        public string Name => _name;

        public bool IsLeaf => true;

        public IEnumerable<ITreeNode> GetChildren(bool refresh)
        {
            return null;
        }

        public bool Sortable => false;

        public string Query => $@"select column_name from SYS.ALL_IND_COLUMNS
where table_owner = '{_table.Schema.Name}' and table_name = '{_table.Name
            }' and index_name = '{_name}'
order by column_position";

        public ContextMenuStrip ContextMenu => null;
        public ContextMenu GetContextMenu()
        {
            throw new System.NotImplementedException();
        }

        private readonly TableNode _table;
        private readonly string _name;
    }
}