using System.Collections.Generic;
using System.Data.SQLite;
using DataCommander.Api;
using Foundation.Assertions;
using Foundation.Data;

namespace DataCommander.Providers.SQLite.ObjectExplorer;

internal sealed class DatabaseCollectionNode : ITreeNode
{
    private readonly SQLiteConnection _connection;

    public DatabaseCollectionNode(SQLiteConnection connection)
    {
        Assert.IsTrue(connection != null);

        _connection = connection;
    }

    #region ITreeNode Members

    string ITreeNode.Name => "Databases";

    bool ITreeNode.IsLeaf => false;

    IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
    {
        const string commandText = @"PRAGMA database_list;";
        var executor = DbCommandExecutorFactory.Create(_connection);
        return executor.ExecuteReader(new ExecuteReaderRequest(commandText), 128, dataRecord =>
        {
            var name = dataRecord.GetString(1);
            return new DatabaseNode(_connection, name);
        });
    }

    bool ITreeNode.Sortable => false;
    string ITreeNode.Query => null;

    public ContextMenu? GetContextMenu()
    {
        throw new System.NotImplementedException();
    }

    #endregion
}