using System.Collections.Generic;
using DataCommander.Providers2;
using Foundation.Data;
using Oracle.ManagedDataAccess.Client;

namespace DataCommander.Providers.Odp.ObjectExplorer;

internal sealed class SchemaCollectionNode : ITreeNode
{
    public SchemaCollectionNode(OracleConnection connection)
    {
        _connection = connection;
        var oracleConnectionStringBuilder = new OracleConnectionStringBuilder(connection.ConnectionString);
        _selectedSchema = oracleConnectionStringBuilder.UserID;
    }

    public string Name => "Schemas";

    public bool IsLeaf => false;

    public IEnumerable<ITreeNode> GetChildren(bool refresh)
    {
        var commandText = "select username from all_users order by username";
        var executor = _connection.CreateCommandExecutor();
        var dataTable = executor.ExecuteDataTable(new ExecuteReaderRequest(commandText));
        var count = dataTable.Rows.Count;
        var treeNodes = new ITreeNode[count];

        for (var i = 0; i < count; i++)
        {
            var name = (string) dataTable.Rows[i][0];
            treeNodes[i] = new SchemaNode(this, name);
        }

        return treeNodes;
    }

    public bool Sortable => false;
    public string Query => null;

    public ContextMenu GetContextMenu()
    {
        throw new System.NotImplementedException();
    }

    public OracleConnection Connection => _connection;

    public string SelectedSchema
    {
        get => _selectedSchema;
        set => _selectedSchema = value;
    }

    readonly OracleConnection _connection;
    string _selectedSchema;
}