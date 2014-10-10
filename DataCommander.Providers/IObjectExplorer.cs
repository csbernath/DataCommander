namespace DataCommander.Providers
{
    using System.Collections.Generic;
    using System.Data;

    public interface IObjectExplorer
    {
        void SetConnection(string connectionString, IDbConnection connection);

        IEnumerable<ITreeNode> GetChildren(bool refresh);

        bool Sortable { get; }
    }
}