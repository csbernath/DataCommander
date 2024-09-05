namespace DataCommander.Api;

public sealed class SqlObject(string? parentName, string? parentAlias, SqlObjectTypes type, string? name)
{
    public string? ParentName { get; set; } = parentName;
    public string? ParentAlias { get; } = parentAlias;
    public SqlObjectTypes Type { get; } = type;
    public string? Name { get; set; } = name;
}