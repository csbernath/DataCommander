using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Windows.Forms;
using DataCommander.Providers.Connection;
using Foundation.Data;

namespace DataCommander.Providers.OracleBase.ObjectExplorer
{
    /// <summary>
    /// Summary description for SchemaNode.
    /// </summary>
    public sealed class SchemaCollectionNode : ITreeNode
    {
        private readonly IDbConnection _connection;
        private string _selectedSchema;

        public SchemaCollectionNode(IDbConnection connection)
        {
            _connection = connection;
            var sb = new DbConnectionStringBuilder();
            sb.ConnectionString = connection.ConnectionString;
            _selectedSchema = (string) sb[ConnectionStringKeyword.UserId];
        }

        public string Name => "Schemas";

        public bool IsLeaf => false;

        public IEnumerable<ITreeNode> GetChildren(bool refresh)
        {
            var commandText = "select username from all_users order by username";
            var executor = Connection.CreateCommandExecutor();
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
        public ContextMenuStrip ContextMenu => null;
        public IDbConnection Connection => _connection;

        public string SelectedSchema
        {
            get => _selectedSchema;
            set => _selectedSchema = value;
        }
    }
}