﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Collections.ReadOnly;
using Foundation.Data;
using Foundation.Data.SqlClient;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class JobNode : ITreeNode
{
    private readonly JobCollectionNode _jobs;
    private readonly string? _name;

    public JobNode(JobCollectionNode jobs, string? name)
    {
        ArgumentNullException.ThrowIfNull(jobs);

        _jobs = jobs;
        _name = name;
    }

    string? ITreeNode.Name => _name;
    bool ITreeNode.IsLeaf => true;

    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken) => throw new NotSupportedException();

    bool ITreeNode.Sortable => false;

    string ITreeNode.Query => null;

    public ContextMenu? GetContextMenu()
    {
        System.Collections.ObjectModel.ReadOnlyCollection<MenuItem> menuItems = new[]
        {
            new MenuItem("HelpJob", OnHelpJobClick, EmptyReadOnlyCollection<MenuItem>.Value)
        }.ToReadOnlyCollection();
        ContextMenu contextMenu = new ContextMenu(menuItems);

        return contextMenu;
    }

    private void OnHelpJobClick(object? sender, EventArgs e)
    {
        string commandText = $@"msdb..sp_help_job @job_name = {_name.ToNullableNVarChar()}";
        DataSet dataSet;

        using (Microsoft.Data.SqlClient.SqlConnection connection = _jobs.Server.CreateConnection())
        {
            IDbCommandExecutor executor = connection.CreateCommandExecutor();
            dataSet = executor.ExecuteDataSet(new ExecuteReaderRequest(commandText), CancellationToken.None);
        }

        IQueryForm? queryForm = (IQueryForm)sender;
        queryForm.ShowDataSet(dataSet);
    }
}