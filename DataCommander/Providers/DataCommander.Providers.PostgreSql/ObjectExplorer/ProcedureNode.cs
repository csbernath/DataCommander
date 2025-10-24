using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Data;
using Foundation.Linq;
using Foundation.Text;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer;

internal sealed class ProcedureNode(
    SchemaNode schemaNode,
    uint oid,
    string? name, 
    IReadOnlyCollection<FunctionArgument> functionArguments) : ITreeNode
{
    public string? Name
    {
        get
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(name);

            if (functionArguments.Count > 0)
            {
                stringBuilder.Append('(');
                var first = true;

                foreach (var functionArgument in functionArguments)
                {
                    if (first)
                        first = false;
                    else
                        stringBuilder.Append(", ");

                    stringBuilder.Append(ToString(functionArgument));
                }

                stringBuilder.Append(')');
            }

            return stringBuilder.ToString();
        }
    }

    bool ITreeNode.IsLeaf => true;

    public Task<IEnumerable<ITreeNode>> GetChildren(bool refresh, CancellationToken cancellationToken) =>
        Task.FromResult<IEnumerable<ITreeNode>>([]);

    bool ITreeNode.Sortable => false;

    public bool DynamicChildCount => false;

    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken) => Task.FromResult<string?>(null);

    public ContextMenu? GetContextMenu()
    {
        var menuItem = new MenuItem("CREATE Script", CreatScriptClicked, []);
        return new ContextMenu([menuItem]);
    }

    private async void CreatScriptClicked(object? sender, EventArgs e)
    {
        var commandText = $@"select prosrc
from pg_proc
where oid = {oid}";
        var scalar = await Db.ExecuteScalarAsync(
            schemaNode.SchemaCollectionNode.DatabaseNode.CreateConnection,
            new CreateCommandRequest(commandText),
            CancellationToken.None);
        var prosrc = (string)scalar!;

        var textBuilder = new TextBuilder();
        textBuilder.Add($"CREATE OR REPLACE PROCEDURE \"{schemaNode.Name}\".\"{name}\"(");

        using (textBuilder.Indent(1))
        {
            var lastIndex = functionArguments.Count - 1;
            foreach (var item in functionArguments.SelectIndexed())
            {
                var stringBuilder = new StringBuilder();
                var functionArgument = item.Value;
                stringBuilder.Append(ToString(functionArgument));
                if (item.Index < lastIndex)
                    stringBuilder.Append(',');
                textBuilder.Add(stringBuilder.ToString());
            }
        }

        textBuilder.AddToLastLine(")");
        textBuilder.Add("LANGUAGE 'plpgsql'");
        textBuilder.Add($"AS $BODY${prosrc}$BODY$;");

        var queryForm = (IQueryForm)sender!;
        queryForm.SetClipboardText(textBuilder.ToLines().ToIndentedString("    "));
        queryForm.SetStatusbarPanelText("Copying script to clipboard finished.");
    }

    private static string ToString(FunctionArgumentMode mode)
    {
        return mode switch
        {
            FunctionArgumentMode.In => "IN",
            FunctionArgumentMode.Out => "OUT",
            FunctionArgumentMode.InOut => "INOUT",
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };
    }

    private string ToString(FunctionArgument functionArgument)
    {
        var type = functionArgument.Type;
        var postgresSqlTypeRepository = schemaNode.SchemaCollectionNode.DatabaseNode.TypeRepository!;

        if (type.ElementType != null)
            type = type.ElementType;
        
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(ToString(functionArgument.Mode));
        stringBuilder.Append(' ');
        stringBuilder.Append(functionArgument.Name);
        stringBuilder.Append(' ');

        if (type.Namespace.Oid != PostgresSqlNamespaceOid.PgCatalog)
        {
            stringBuilder.Append(' ');
            stringBuilder.Append($"\"{type.Namespace.Name}\"");
            stringBuilder.Append('.');            
        }
        else
            stringBuilder.Append(' ');

        var typeName = postgresSqlTypeRepository.TryGetPostgresSqlTypeName(type.Oid, out var postgresSqlTypeName)
            ? postgresSqlTypeName!.Name
            : type.Name;

        stringBuilder.Append($"\"{typeName}\"");

        if (functionArgument.Type.Category == TypeCategory.ArrayTypes)
            stringBuilder.Append("[]");

        if (functionArgument.Type.Oid == PostgresSqlTypeOid.RefCursor)
            stringBuilder.Append($" DEFAULT '{functionArgument.Name}'::refcursor");
        
        return stringBuilder.ToString();
    }
}