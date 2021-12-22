namespace DataCommander.Providers2;

public sealed class SqlObject
{
    public SqlObject(string parentName, string parentAlias, SqlObjectTypes type, string name)
    {
        ParentName = parentName;
        ParentAlias = parentAlias;
        Type = type;
        Name = name;
    }

    public string ParentName { get; set; }
    public string ParentAlias { get; }
    public SqlObjectTypes Type { get; }
    public string Name { get; set; }
}