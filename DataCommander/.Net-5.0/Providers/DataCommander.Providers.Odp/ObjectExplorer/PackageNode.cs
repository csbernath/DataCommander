﻿using System;
using System.Collections.Generic;
using System.Text;
using DataCommander.Api;
using Foundation.Collections.ReadOnly;
using Foundation.Data;

namespace DataCommander.Providers.Odp.ObjectExplorer;

internal sealed class PackageNode : ITreeNode
{
    private readonly SchemaNode _schemaNode;
    private readonly string _name;

    public PackageNode(SchemaNode schema, string name)
    {
        _schemaNode = schema;
        _name = name;
    }

    public string Name => _name;
    public bool IsLeaf => false;

    public IEnumerable<ITreeNode> GetChildren(bool refresh)
    {
        //string commandText = "select distinct object_name from all_arguments where owner='{0}' and package_name='{1}'";
        //commandText = string.Format(commandText, schema.Name, name);
        var commandText =
            $@"select	procedure_name
from	all_procedures
where	owner = '{_schemaNode.Name}'
	and object_name = '{_name}'
order by procedure_name";
        var executor = _schemaNode.SchemasNode.Connection.CreateCommandExecutor();

        return executor.ExecuteReader(new ExecuteReaderRequest(commandText), 128, dataRecord =>
        {
            var procedureName = dataRecord.GetString(0);
            return new ProcedureNode(_schemaNode, this, procedureName);
        });
    }

    public bool Sortable => false;

    public string Query
    {
        get
        {
            var commandText = "select text from all_source where owner = '{0}' and type = 'PACKAGE' and name = '{1}'";
            commandText = string.Format(commandText, _schemaNode.Name, _name);
            var executor = _schemaNode.SchemasNode.Connection.CreateCommandExecutor();
            var dataTable = executor.ExecuteDataTable(new ExecuteReaderRequest(commandText));
            var sb = new StringBuilder();

            for (var i = 0; i < dataTable.Rows.Count; i++)
            {
                var dataRow = dataTable.Rows[i];
                sb.Append(dataRow[0]);
            }

            return sb.ToString();
        }
    }

    private void ScriptPackage(object sender, EventArgs e)
    {
    }

    private void ScriptPackageBody(object sender, EventArgs e)
    {
        var commandText = "select text from all_source where owner = '{0}' and name = '{1}' and type = 'PACKAGE BODY'";
        commandText = string.Format(commandText, _schemaNode.Name, _name);
        var executor = _schemaNode.SchemasNode.Connection.CreateCommandExecutor();
        var dataTable = executor.ExecuteDataTable(new ExecuteReaderRequest(commandText));
        var dataRows = dataTable.Rows;
        var count = dataRows.Count;
        var sb = new StringBuilder();

        for (var i = 0; i < count; i++)
        {
            var dataRow = dataRows[i];
            var line = (string)dataRow[0];
            sb.Append(line);
        }

        var queryForm = (IQueryForm)sender;
        var append = sb.ToString();
        queryForm.ShowText(append);
    }

    public ContextMenu? GetContextMenu()
    {
        var menuItemPackage = new MenuItem("Script Package", ScriptPackage, EmptyReadOnlyCollection<MenuItem>.Value);
        var menuItemPackageBody = new MenuItem("Script Package Body", ScriptPackageBody, EmptyReadOnlyCollection<MenuItem>.Value);
        var items = new[] { menuItemPackage, menuItemPackageBody }.ToReadOnlyCollection();
        var contextMenu = new ContextMenu(items);
        return contextMenu;
    }

    public SchemaNode SchemaNode => _schemaNode;
}