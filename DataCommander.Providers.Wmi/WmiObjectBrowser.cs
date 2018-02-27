namespace DataCommander.Providers.Wmi
{
    using System.Collections.Generic;
    using System.Data;

    sealed class WmiObjectExplorer : IObjectExplorer
    {
        private WmiConnection connection;

        void IObjectExplorer.SetConnection( string connectionString, IDbConnection connection )
        {
            this.connection = (WmiConnection) connection;
        }

        public IEnumerable<ITreeNode> GetChildren( bool refresh )
        {
            return new ITreeNode[]
            {
                new WmiClasses(connection.Scope)
            };
        }

        public bool Sortable => false;
    }
}