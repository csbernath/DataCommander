using System;
using System.Collections.Generic;
using System.Text;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer;

internal static class FunctionArgumentExtensions
{
    public static string Script(this FunctionArgument functionArgument, PostgresSqlTypeRepository postgresSqlTypeRepository)
    {
        var type = functionArgument.Type;

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
            : $"\"{type.Name}\"";

        stringBuilder.Append(typeName);

        if (functionArgument.Type.Category == TypeCategory.ArrayTypes)
            stringBuilder.Append("[]");

        if (functionArgument.Type.Oid == PostgresSqlTypeOid.RefCursor)
            stringBuilder.Append($" DEFAULT '{functionArgument.Name}'::refcursor");

        return stringBuilder.ToString();
    }
    
    public static string GetRoutineName(
        string name,
        IReadOnlyCollection<FunctionArgument> functionArguments,
        PostgresSqlTypeRepository postgresSqlTypeRepository)
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

                    stringBuilder.Append(functionArgument.Script(postgresSqlTypeRepository));
                }

                stringBuilder.Append(')');
            }

            return stringBuilder.ToString();
    }
    
    public static string ToString(FunctionArgumentMode mode)
    {
        return mode switch
        {
            FunctionArgumentMode.In => "IN",
            FunctionArgumentMode.Out => "OUT",
            FunctionArgumentMode.InOut => "INOUT",
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };
    }    
}