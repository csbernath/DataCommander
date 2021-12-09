using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;

namespace DataCommander.Providers.SqlServerCe40.ObjectExplorer;

internal sealed class SqlCeObjectExplorer : IObjectExplorer
{
    private SqlCeConnection _connection;

    public string ConnectionString { get; private set; }

    #region IObjectExplorer Members

    void IObjectExplorer.SetConnection(string connectionString, IDbConnection connection)
    {
        ConnectionString = connectionString;
        _connection = (SqlCeConnection) connection;
    }

    IEnumerable<ITreeNode> IObjectExplorer.GetChildren(bool refresh)
    {
        yield return new TableCollectionNode(this, _connection);
    }

    bool IObjectExplorer.Sortable => false;

    #endregion
}