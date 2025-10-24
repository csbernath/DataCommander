namespace DataCommander.Providers.PostgreSql.ObjectExplorer;

public class PostgresSqlNamespace(uint oid, string name)
{
    public readonly uint Oid = oid;
    public readonly string Name = name;
}