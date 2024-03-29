﻿using System.Collections.Generic;
using DataCommander.Api;
using Foundation.Data;

namespace DataCommander.Providers.Odp.ObjectExplorer;

internal sealed class FunctionCollectionNode : ITreeNode
{
    private readonly SchemaNode _schemaNode;

    public FunctionCollectionNode(SchemaNode schemaNode)
    {
        _schemaNode = schemaNode;
    }

    #region ITreeNode Members

    string ITreeNode.Name => "Functions";

    bool ITreeNode.IsLeaf => false;

    IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
    {
        var commandText = $@"select	OBJECT_NAME
from SYS.ALL_OBJECTS
where OWNER	= '{_schemaNode.Name}'
    and OBJECT_TYPE	= 'FUNCTION'
order by OBJECT_NAME";


        var executor = _schemaNode.SchemasNode.Connection.CreateCommandExecutor();
        var functionNodes = executor.ExecuteReader(new ExecuteReaderRequest(commandText), 128, dataRecord =>
        {
            var procedureName = dataRecord.GetString(0);
            return new FunctionNode(_schemaNode, null, procedureName);
        });
        return functionNodes;
    }

    bool ITreeNode.Sortable => false;
    string ITreeNode.Query => null;

    public ContextMenu? GetContextMenu()
    {
        throw new System.NotImplementedException();
    }

    #endregion
}