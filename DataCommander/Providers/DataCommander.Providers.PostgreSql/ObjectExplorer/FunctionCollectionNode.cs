using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Data;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer;

internal sealed class FunctionCollectionNode(SchemaNode schemaNode) : ITreeNode
{
    bool ITreeNode.IsLeaf => false;
    string ITreeNode.Name => "Functions";

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
    proargtypes,
    proallargtypes 
from pg_proc
where
    prokind = 'f' and
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
                var proargmodes = dataRecord.GetNullableCharArray(2);
                var proargnames = dataRecord.GetNullableStringArray(3);
                var proargtypes = dataRecord.GetNullableUInt32Array(4);
                var proallargtypes = dataRecord.GetNullableUInt32Array(5);

                var functionArguments = new List<FunctionArgument>();

                if (proargnames != null)
                {
                    var typeRepository = schemaNode.SchemaCollectionNode.DatabaseNode.TypeRepository!;

                    for (var index = 0; index < proargnames.Length; ++index)
                    {
                        var mode = ToFunctionArgumentMode(proargmodes, index);
                        var argumentName = proargnames[index];
                        var typeOid = GetTypeOid(proargtypes, proallargtypes, index);
                        typeRepository.TryGetByOid(typeOid, out var type);
                        var functionArgument = new FunctionArgument(mode, argumentName, type!);
                        functionArguments.Add(functionArgument);
                    }
                }

                return new FunctionNode(schemaNode, oid, name, functionArguments);
            },
            cancellationToken);
        return procedureNodes;
    }

    private static FunctionArgumentMode ToFunctionArgumentMode(char[]? argmodes, int index)
    {
        FunctionArgumentMode functionArgumentMode;
        if (argmodes != null)
        {
            functionArgumentMode = argmodes[index] switch
            {
                'i' => FunctionArgumentMode.In,
                'b' => FunctionArgumentMode.InOut,
                'o' => FunctionArgumentMode.Out,
                _ => throw new NotImplementedException()
            };
        }
        else
            functionArgumentMode = FunctionArgumentMode.In;

        return functionArgumentMode;
    }

    private static uint GetTypeOid(uint[]? proargtypes, uint[]? proallargtypes, int index)
    {
        var typeOid = proallargtypes != null
            ? proallargtypes[index]
            : proargtypes![index];
        return typeOid;
    }
}