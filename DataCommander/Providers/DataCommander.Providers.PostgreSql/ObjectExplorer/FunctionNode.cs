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

internal sealed class FunctionNode(
    SchemaNode schemaNode,
    uint oid,
    string name, 
    IReadOnlyCollection<FunctionArgument> functionArguments) : ITreeNode
{
    public string Name => FunctionArgumentExtensions.GetRoutineName(name, functionArguments, schemaNode.SchemaCollectionNode.DatabaseNode.TypeRepository);

    bool ITreeNode.IsLeaf => true;

    public IReadOnlyCollection<string> GetFilterableProperties() => [];

    public Task<IEnumerable<ITreeNode>> GetChildren(IReadOnlyCollection<FilterCriterion> filterCriteria, bool refresh, CancellationToken cancellationToken) =>
        Task.FromResult<IEnumerable<ITreeNode>>([]);

    public IReadOnlyCollection<FilterCriterion> GetFilterCriteria() => [];

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
        var typeRepository = schemaNode.SchemaCollectionNode.DatabaseNode.TypeRepository;
        
        var commandText = $@"select prosrc
from pg_proc
where oid = {oid}";
        var scalar = await Db.ExecuteScalarAsync(
            schemaNode.SchemaCollectionNode.DatabaseNode.CreateConnection,
            new CreateCommandRequest(commandText),
            CancellationToken.None);
        var prosrc = (string)scalar!;

        var textBuilder = new TextBuilder();
        textBuilder.Add($"CREATE OR REPLACE FUNCTION \"{schemaNode.Name}\".\"{name}\"(");

        using (textBuilder.Indent(1))
        {
            var lastIndex = functionArguments.Count - 1;
            foreach (var item in functionArguments.SelectIndexed())
            {
                var stringBuilder = new StringBuilder();
                var functionArgument = item.Value;
                stringBuilder.Append(functionArgument.Script(typeRepository));
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
}