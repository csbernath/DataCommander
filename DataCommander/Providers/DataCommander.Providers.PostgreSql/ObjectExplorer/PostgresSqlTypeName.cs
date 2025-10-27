using System;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer;

[CLSCompliant(false)]
public class PostgresSqlTypeName(PostgresSqlType type, string name)
{
    public readonly PostgresSqlType Type = type;
    public readonly string Name = name;
}