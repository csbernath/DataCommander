using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;
using Foundation.Assertions;
using Foundation.Core;

namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    internal sealed class ServerNode : ITreeNode
    {
        public ServerNode(string connectionString)
        {
            Assert.IsTrue(!connectionString.IsNullOrWhiteSpace());

            ConnectionString = connectionString;
        }

        public string ConnectionString { get; }

        #region ITreeNode Members

        string ITreeNode.Name
        {
            get
            {
                string serverVersion;
                using (var connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    serverVersion = new Version(connection.ServerVersion).ToString();
                }

                var csb = new SqlConnectionStringBuilder(ConnectionString);
                string userName;
                if (csb.IntegratedSecurity)
                    userName = Environment.UserDomainName + "\\" + Environment.UserName;
                else
                    userName = csb.UserID;

                return $"{csb.DataSource}(SQL Server {serverVersion} - {userName})";
            }
        }

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var node = new DatabaseCollectionNode(this);
            var securityNode = new SecurityNode(this);
            var serverObjectCollectionNode = new ServerObjectCollectionNode(this);
            var jobCollectionNode = new JobCollectionNode(this);
            return new ITreeNode[] {node, securityNode, serverObjectCollectionNode, jobCollectionNode};
        }

        bool ITreeNode.Sortable => false;
        string ITreeNode.Query => null;
        ContextMenuStrip ITreeNode.ContextMenu => null;
        public ContextMenu GetContextMenu()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}