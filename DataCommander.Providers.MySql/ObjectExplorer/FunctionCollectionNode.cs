using System.Collections.Generic;
using Foundation.Data;
using Foundation.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace DataCommander.Providers.MySql.ObjectExplorer
{
    internal sealed class FunctionCollectionNode : ITreeNode
    {
        private readonly DatabaseNode databaseNode;

        public FunctionCollectionNode(DatabaseNode databaseNode)
        {
            this.databaseNode = databaseNode;
        }

        string ITreeNode.Name => "Functions";

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var commandText =
                $@"select r.ROUTINE_NAME
from information_schema.ROUTINES r
where
    r.ROUTINE_SCHEMA = {databaseNode.Name.ToTSqlVarChar()}
    and r.ROUTINE_TYPE = 'FUNCTION'
order by r.ROUTINE_NAME";

            return MySqlClientFactory.Instance.ExecuteReader(
                databaseNode.ObjectExplorer.ConnectionString,
                new ExecuteReaderRequest(commandText),
                dataRecord =>
                {
                    var name = dataRecord.GetString(0);
                    return new FunctionNode(databaseNode, name);
                });
        }

        bool ITreeNode.Sortable => false;
        string ITreeNode.Query => null;
        System.Windows.Forms.ContextMenuStrip ITreeNode.ContextMenu => null;
    }
}