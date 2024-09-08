using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using DataCommander.Api.Connection;
using Foundation.Data;

namespace DataCommander.Providers.SQLite.ObjectExplorer;

internal sealed class DatabaseCollectionNode : ITreeNode
{
    private readonly ConnectionStringAndCredential _connectionStringAndCredential;

    public DatabaseCollectionNode(ConnectionStringAndCredential connectionStringAndCredential)
    {
        _connectionStringAndCredential = connectionStringAndCredential;
    }

    public ConnectionStringAndCredential ConnectionStringAndCredential => _connectionStringAndCredential;

    #region ITreeNode Members

    string? ITreeNode.Name => "Databases";

    bool ITreeNode.IsLeaf => false;

    public async Task<IEnumerable<ITreeNode>> GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        const string commandText = @"PRAGMA database_list;";
        
        return await Db.ExecuteReaderAsync(
            () => ConnectionFactory.CreateConnection(_connectionStringAndCredential),
            new ExecuteReaderRequest(commandText),
            1,
            dataRecord =>
            {
                var name = dataRecord.GetString(1);
                return new DatabaseNode(this, name);
            },cancellationToken);
    }

    bool ITreeNode.Sortable => false;
    string ITreeNode.Query => null;

    public ContextMenu? GetContextMenu()
    {
        throw new System.NotImplementedException();
    }

    #endregion
}