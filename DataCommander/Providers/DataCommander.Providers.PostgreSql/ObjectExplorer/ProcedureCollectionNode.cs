using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Data;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer;

internal sealed class ProcedureCollectionNode(SchemaNode schemaNode) : ITreeNode
{
    bool ITreeNode.IsLeaf => false;
    string ITreeNode.Name => "Procedures";

    public bool DynamicChildCount => true;

    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken) => Task.FromResult<string?>(null);
    public ContextMenu? GetContextMenu() => null;

    bool ITreeNode.Sortable => false;

    public IReadOnlyCollection<string> GetFilterableProperties() => [];

    public async Task<IEnumerable<ITreeNode>> GetChildren(IReadOnlyList<FilterCriterion> filterCriteria, bool refresh, CancellationToken cancellationToken)
    {
        var commandText = @$"select
    oid,
    proname,
    proargmodes,
	proargnames,
    proallargtypes 
from pg_proc
where
    prokind = 'p' and
    pronamespace = {schemaNode.Oid}
order by proname";
        var procedureNodes = await Db.ExecuteReaderAsync(
            schemaNode.SchemaCollectionNode.DatabaseNode.CreateConnection,
            new ExecuteReaderRequest(commandText),
            128,
            dataRecord =>
            {
                var oid = dataRecord.GetUInt32(0);
                var name = dataRecord.GetString(1);
                var argmodes = dataRecord.GetNullableCharArray(2);
                var argnames = dataRecord.GetNullableStringArray(3);
                var allargtypes = dataRecord.GetNullableUInt32Array(4);

                var functionArguments = new List<FunctionArgument>();

                if (argnames != null)
                {
                    var typeRepository = schemaNode.SchemaCollectionNode.DatabaseNode.TypeRepository!;
                    
                    for (var index = 0; index < argnames.Length; ++index)
                    {
                        var mode = ToFunctionArgumentMode(argmodes![index]);
                        var argumentName = argnames[index];
                        var typeOid = allargtypes![index];
                        typeRepository.TryGetByOid(typeOid, out var type);
                        var functionArgument = new FunctionArgument(mode, argumentName, type!);
                        functionArguments.Add(functionArgument);
                    }
                }

                return new ProcedureNode(schemaNode, oid, name, functionArguments);
            },
            cancellationToken);
        return procedureNodes;
    }

    private static FunctionArgumentMode ToFunctionArgumentMode(char argmode)
    {
        var mode = argmode switch
        {
            'i' => FunctionArgumentMode.In,
            'b' => FunctionArgumentMode.InOut,
            'o' => FunctionArgumentMode.Out,
            _ => throw new NotImplementedException()
        };
        return mode;
    }
}