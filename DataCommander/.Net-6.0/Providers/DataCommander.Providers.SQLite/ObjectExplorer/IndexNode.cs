﻿using System.Collections.Generic;
using DataCommander.Api;
using Foundation.Data;

namespace DataCommander.Providers.SQLite.ObjectExplorer;

internal sealed class IndexNode : ITreeNode
{
    private readonly TableNode _tableNode;
    private readonly string _name;

    public IndexNode(TableNode tableNode, string name)
    {
        _tableNode = tableNode;
        _name = name;
    }

    #region ITreeNode Members

    string ITreeNode.Name => _name;

    bool ITreeNode.IsLeaf => true;

    IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
    {
        return null;
    }

    bool ITreeNode.Sortable => false;

    string ITreeNode.Query
    {
        get
        {
            var commandText = $@"select sql
from main.sqlite_master
where
    type = 'index'
    and name = '{_name}'";
            var executor = DbCommandExecutorFactory.Create(_tableNode.Database.Connection);
            var sql = (string) executor.ExecuteScalar(new CreateCommandRequest(commandText));
            return sql;
        }
    }

    public ContextMenu? GetContextMenu()
    {
        throw new System.NotImplementedException();
    }

    #endregion
}