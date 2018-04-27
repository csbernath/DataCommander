using System.Collections.Generic;
using System.Data;

namespace DataCommander.Providers.Msi
{
    internal sealed class MsiObjectExplorer : IObjectExplorer
    {
        private MsiConnection _connection;

        #region IObjectExplorer Members

        void IObjectExplorer.SetConnection(string connectionString, IDbConnection connection)
        {
            _connection = (MsiConnection) connection;
        }

        IEnumerable<ITreeNode> IObjectExplorer.GetChildren(bool refresh)
        {
            return new ITreeNode[] {new MsiTableCollectionNode(_connection)};
        }

        bool IObjectExplorer.Sortable => false;

        #endregion
    }
}