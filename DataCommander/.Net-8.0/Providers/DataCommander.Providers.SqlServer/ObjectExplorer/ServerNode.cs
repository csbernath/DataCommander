using System;
using System.Collections.Generic;
using DataCommander.Api;
using Microsoft.Data.SqlClient;
using Foundation.Assertions;
using Foundation.Core;
using Foundation.Data;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

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
            var connectionStringBuilder = new SqlConnectionStringBuilder(ConnectionString);

            string serverVersion;
            string userName = null;
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                serverVersion = new Version(connection.ServerVersion).ToString();

                if (connectionStringBuilder.IntegratedSecurity)
                {
                    var commanExecutor = connection.CreateCommandExecutor();
                    const string commandText = "select suser_sname()";
                    var createCommandRequest = new CreateCommandRequest(commandText);
                    var scalar = commanExecutor.ExecuteScalar(createCommandRequest);
                    userName = (string)scalar;
                }
            }

            if (!connectionStringBuilder.IntegratedSecurity)
                userName = connectionStringBuilder.UserID;

            return $"{connectionStringBuilder.DataSource}(SQL Server {serverVersion} - {userName})";
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

    public ContextMenu? GetContextMenu() => null;

    #endregion
}