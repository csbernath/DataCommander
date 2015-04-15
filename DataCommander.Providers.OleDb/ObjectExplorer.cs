namespace DataCommander.Providers.OleDb
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.OleDb;

    /// <summary>
    /// Summary description for ObjectBrowser.
    /// </summary>
    internal sealed class ObjectExplorer : IObjectExplorer
    {
        public IEnumerable<ITreeNode> GetChildren(bool refresh)
        {
            ITreeNode[] treeNodes = new ITreeNode[1];
            treeNodes[0] = new CatalogsNode(this.connection);
            return treeNodes;
        }

        public bool Sortable
        {
            get
            {
                return false;
            }
        }

        private OleDbConnection connection;

        #region IObjectExplorer Members

        void IObjectExplorer.SetConnection(string connectionString, IDbConnection connection)
        {
            this.connection = (OleDbConnection) connection;
        }

        #endregion
    }
}