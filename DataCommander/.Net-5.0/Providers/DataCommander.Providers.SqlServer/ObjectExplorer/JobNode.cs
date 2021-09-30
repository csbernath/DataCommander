using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;
using DataCommander.Providers.Query;
using Foundation.Assertions;
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

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            throw new NotImplementedException();
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query
        {
            get
            {
                var commandText = $@"msdb..sp_help_job @job_name = {_name.ToNullableNVarChar()}";
                DataSet dataSet;

                using (var connection = new SqlConnection(_jobs.Server.ConnectionString))
                {
                    var executor = connection.CreateCommandExecutor();
                    dataSet = executor.ExecuteDataSet(new ExecuteReaderRequest(commandText));
                }

                var queryForm = (QueryForm) DataCommanderApplication.Instance.MainForm.ActiveMdiChild;
                queryForm.ShowDataSet(dataSet);

                return null;
            }
        }

        ContextMenuStrip ITreeNode.ContextMenu => null;

        #endregion
    }
}