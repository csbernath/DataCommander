using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using Microsoft.Data.SqlClient;
using DataCommander.Providers.Query;
using Foundation.Assertions;
using Foundation.Collections.ReadOnly;
using Foundation.Data;
using Foundation.Data.SqlClient;

namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    internal sealed class JobNode : ITreeNode
    {
        private readonly JobCollectionNode _jobs;
        private readonly string _name;

        public JobNode(JobCollectionNode jobs, string name)
        {
            Assert.IsNotNull(jobs);

            _jobs = jobs;
            _name = name;
        }

        #region ITreeNode Members

        string ITreeNode.Name => _name;
        bool ITreeNode.IsLeaf => true;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh) => throw new NotSupportedException();

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => null;

        public ContextMenu GetContextMenu()
        {
            var menuItems = new[]
            {
                new MenuItem("HelpJob", OnHelpJobClick, EmptyReadOnlyCollection<MenuItem>.Value)
            }.ToReadOnlyCollection();
            var contextMenu = new ContextMenu(menuItems);

            return contextMenu;
        }

        private void OnHelpJobClick(object? sender, EventArgs e)
        {
            var commandText = $@"msdb..sp_help_job @job_name = {_name.ToNullableNVarChar()}";
            DataSet dataSet;

            using (var connection = new SqlConnection(_jobs.Server.ConnectionString))
            {
                var executor = connection.CreateCommandExecutor();
                dataSet = executor.ExecuteDataSet(new ExecuteReaderRequest(commandText));
            }

            var queryForm = (IQueryForm)sender;
            queryForm.ShowDataSet(dataSet);
        }

        #endregion
    }
}