namespace DataCommander.Providers.PostgreSql.ObjectExplorer;

public class PostgresSqlType(uint oid, string name)
{
    public readonly uint Oid = oid;
    public readonly string Name = name;
}