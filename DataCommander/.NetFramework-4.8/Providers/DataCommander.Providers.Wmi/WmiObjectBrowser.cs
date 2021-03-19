using System.Collections.Generic;
using System.Data;

namespace DataCommander.Providers.Wmi
{
    sealed class WmiObjectExplorer : IObjectExplorer
    {
        private WmiConnection _connection;

        void IObjectExplorer.SetConnection( string connectionString, IDbConnection connection )
        {
            _connection = (WmiConnection) connection;
        }

        public IEnumerable<ITreeNode> GetChildren( bool refresh )
        {
            return new ITreeNode[]
            {
                new WmiClasses(_connection.Scope)
            };
        }

        public bool Sortable => false;
    }
}