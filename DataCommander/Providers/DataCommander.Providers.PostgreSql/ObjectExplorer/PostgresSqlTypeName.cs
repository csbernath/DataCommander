namespace DataCommander.Providers.PostgreSql.ObjectExplorer;

public class PostgresSqlTypeName(PostgresSqlType type, string name)
{
    public readonly PostgresSqlType Type = type;
    public readonly string Name = name;
}