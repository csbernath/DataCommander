using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;
using Foundation.Assertions;
using Foundation.Data;
using Foundation.Diagnostics.Contracts;

namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    internal sealed class DatabaseCollectionNode : ITreeNode
    {
        public DatabaseCollectionNode(ServerNode server)
        {
            Assert.IsTrue(server != null);

            Server = server;
        }

        public ServerNode Server { get; }

        #region ITreeNode Members

        string ITreeNode.Name => "Databases";

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var list = new List<ITreeNode>();
            list.Add(new SystemDatabaseCollectionNode(this));

            const string commandText = @"select d.name
from sys.databases d (nolock)
where name not in('master','model','msdb','tempdb')
order by d.name";

            var rows = SqlClientFactory.Instance.ExecuteReader(Server.ConnectionString, new ExecuteReaderRequest(commandText), dataRecord =>
            {
                var name = dataRecord.GetString(0);
                return new DatabaseNode(this, name);
            });
            list.AddRange(rows);
            return list;
        }

        bool ITreeNode.Sortable => false;
        string ITreeNode.Query => null;
        ContextMenuStrip ITreeNode.ContextMenu => null;

        #endregion
    }
}