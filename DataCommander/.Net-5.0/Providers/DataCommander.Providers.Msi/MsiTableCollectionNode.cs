using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DataCommander.Providers.Msi
{
    internal sealed class MsiTableCollectionNode : ITreeNode
    {
        private readonly MsiConnection _connection;

        public MsiTableCollectionNode(MsiConnection connection)
        {
            _connection = connection;
        }

        #region ITreeNode Members

        string ITreeNode.Name => "Tables";

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var e = from table in _connection.Database.Tables
                    select (ITreeNode)new MsiTableNode(_connection, table);

            return e;
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => null;

        ContextMenuStrip ITreeNode.ContextMenu => null;

        #endregion
    }
}
