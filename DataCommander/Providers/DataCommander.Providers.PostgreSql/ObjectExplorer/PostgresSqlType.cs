using System;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer;

[CLSCompliant(false)]
public class PostgresSqlType(
    uint oid,
    string name,
    PostgresSqlNamespace @namespace,
    TypeCategory category,
    PostgresSqlType? elementType)
{
    public readonly uint Oid = oid;
    public readonly string Name = name;
    public readonly PostgresSqlNamespace Namespace = @namespace;
    public readonly TypeCategory Category = category;
    public PostgresSqlType? ElementType = elementType;
}