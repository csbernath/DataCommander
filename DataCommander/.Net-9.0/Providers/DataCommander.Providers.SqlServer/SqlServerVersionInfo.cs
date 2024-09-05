namespace DataCommander.Providers.SqlServer;

internal sealed class SqlServerVersionInfo(string version, string name)
{
    public readonly string Version = version;
    public readonly string Name = name;
}