using System;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer;

[CLSCompliant(false)]
public class PostgresSqlNamespace(uint oid, string name)
{
    public readonly uint Oid = oid;
    public readonly string Name = name;
}