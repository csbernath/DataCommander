﻿using System.Collections.Generic;
using DataCommander.Api;
using Foundation.Data;

namespace DataCommander.Providers.Odp.ObjectExplorer;

internal sealed class PackageCollectionNode : ITreeNode
{
    private readonly SchemaNode _schema;

    public PackageCollectionNode(SchemaNode schema) => _schema = schema;

    public string Name => "Packages";
    public bool IsLeaf => false;

    public IEnumerable<ITreeNode> GetChildren(bool refresh)
    {
        // var folder = DataCommanderApplication.Instance.ApplicationData.CurrentType;
        // var key = _schema.SchemasNode.Connection.DataSource + "." + _schema.Name;
        // var contains = folder.Attributes.TryGetAttributeValue(key, out string[] packages);

        string[] packages;

        //if (!contains || refresh)
        if (true || refresh)
        {
            var commandText = "select object_name from all_objects where owner = '{0}' and object_type = 'PACKAGE' order by object_name";
            commandText = string.Format(commandText, _schema.Name);
            var executor = _schema.SchemasNode.Connection.CreateCommandExecutor();
            var dataTable = executor.ExecuteDataTable(new ExecuteReaderRequest(commandText));
            var count = dataTable.Rows.Count;
            packages = new string[count];

            for (var i = 0; i < count; i++)
                packages[i] = (string) dataTable.Rows[i][0];

            //folder.Attributes.SetAttributeValue(key, packages);
        }

        var treeNodes = new ITreeNode[packages.Length];

        for (var i = 0; i < packages.Length; i++)
            treeNodes[i] = new PackageNode(_schema, packages[i]);

        return treeNodes;
    }

    public bool Sortable => false;
    public string Query => null;
    public SchemaNode Schema => _schema;
    public ContextMenu? GetContextMenu() => null;
}