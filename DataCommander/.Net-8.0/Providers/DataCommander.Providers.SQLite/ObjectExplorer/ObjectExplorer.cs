using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using DataCommander.Api;

namespace DataCommander.Providers.SQLite.ObjectExplorer;

internal sealed class ObjectExplorer : IObjectExplorer
{
    private SQLiteConnection _connection;

    #region IObjectExplorer Members

    void IObjectExplorer.SetConnection(string connectionString, IDbConnection connection)
    {
        _connection = (SQLiteConnection)connection;
    }

    IEnumerable<ITreeNode> IObjectExplorer.GetChildren(bool refresh)
    {
        return new ITreeNode[]
        {
            new DatabaseCollectionNode(_connection)
        };
    }

    bool IObjectExplorer.Sortable => false;

    #endregion
}