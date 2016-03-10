namespace DataCommander.Providers.PostgreSql.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Data;

    internal sealed class ObjectExplorer : IObjectExplorer
    {
        private string connectionString;

        public string ConnectionString => this.connectionString;

        void IObjectExplorer.SetConnection(string connectionString, IDbConnection connection)
        {
            this.connectionString = connectionString;
        }

        IEnumerable<ITreeNode> IObjectExplorer.GetChildren(bool refresh)
        {
            return new[] {new SchemaCollectionNode(this)};
        }

        bool IObjectExplorer.Sortable
        {
            get
            {
                return false;
            }
        }
    }
}