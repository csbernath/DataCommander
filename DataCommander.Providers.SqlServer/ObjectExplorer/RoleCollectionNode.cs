using System.Collections.Generic;
using System.Windows.Forms;
using Foundation.Data;
using Foundation.Data.SqlClient;

namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    internal sealed class RoleCollectionNode : ITreeNode
    {
        private readonly DatabaseNode _database;

        public RoleCollectionNode(DatabaseNode database)
        {
            _database = database;
        }

        public string Name => "Roles";
        public bool IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var commandText = "select name from {0}..sysusers where issqlrole = 1 order by name";
            commandText = string.Format(commandText, this._database.Name);
            var connectionString = this._database.Databases.Server.ConnectionString;
            var executor = new SqlCommandExecutor(connectionString);
            var roleNodes = executor.ExecuteReader(new ExecuteReaderRequest(commandText), dataRecord =>
            {
                var name = dataRecord.GetString(0);
                return new RoleNode(this._database, name);
            });
            return roleNodes;
        }

        public bool Sortable => false;
        public string Query => null;
        public ContextMenuStrip ContextMenu => null;
    }
}