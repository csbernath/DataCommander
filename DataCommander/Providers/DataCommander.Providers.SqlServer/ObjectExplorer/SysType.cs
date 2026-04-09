namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class SysType
{
    public readonly string Name;
    public readonly int UserTypeId;

    public SysType(string name, int userTypeId)
    {
        Name = name;
        UserTypeId = userTypeId;
    }
}