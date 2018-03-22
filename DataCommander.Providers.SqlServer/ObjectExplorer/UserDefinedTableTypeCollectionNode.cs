﻿using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;
using Foundation.Data;

namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    internal sealed class UserDefinedTableTypeCollectionNode : ITreeNode
    {
        private readonly DatabaseNode _database;

        public UserDefinedTableTypeCollectionNode(DatabaseNode database) => _database = database;

        string ITreeNode.Name => "User-Defined Table Types";
        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var commandText = $@"select
    s.name,
    t.name,
    type_table_object_id
from [{_database.Name}].sys.schemas s (nolock)
join [{_database.Name}].sys.table_types t (nolock)
    on s.schema_id = t.schema_id
order by 1,2";

            var tableTypeNodes = new List<UserDefinedTableTypeNode>();
            var connectionString = _database.Databases.Server.ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var executor = connection.CreateCommandExecutor();
                executor.ExecuteReader(new ExecuteReaderRequest(commandText), dataReader =>
                {
                    dataReader.ReadResult(() =>
                    {
                        var schema = dataReader.GetString(0);
                        var name = dataReader.GetString(1);
                        var id = dataReader.GetInt32(2);
                        var tableTypeNode = new UserDefinedTableTypeNode(_database, id, schema, name);
                        tableTypeNodes.Add(tableTypeNode);
                    });
                });
            }

            return tableTypeNodes;
        }

        bool ITreeNode.Sortable => false;
        string ITreeNode.Query => null;
        ContextMenuStrip ITreeNode.ContextMenu => null;
    }
}