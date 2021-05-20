using Foundation.Assertions;
using Foundation.Data;
using Foundation.Data.SqlClient;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    internal sealed class LoginCollectionNode : ITreeNode
    {
        private readonly ServerNode _server;

        public LoginCollectionNode(ServerNode server)
        {
            Assert.IsNotNull(server);
            _server = server;
        }

        string ITreeNode.Name => "Logins";
        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            const string commandText = @"select name
from sys.server_principals sp (nolock)
where   sp.type in('S','U','G')
order by name";
            var request = new ExecuteReaderRequest(commandText);
            var executor = new SqlCommandExecutor(_server.ConnectionString);
            return executor.ExecuteReader(request, 128, dataRecord => new LoginNode(dataRecord.GetString(0)));
        }

        bool ITreeNode.Sortable => false;
        string ITreeNode.Query => null;
        ContextMenuStrip ITreeNode.ContextMenu => null;
    }
}