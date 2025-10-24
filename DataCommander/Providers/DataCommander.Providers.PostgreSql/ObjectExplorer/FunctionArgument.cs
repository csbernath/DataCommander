namespace DataCommander.Providers.PostgreSql.ObjectExplorer;

public class FunctionArgument(FunctionArgumentMode mode, string name, PostgresSqlType type)
{
    public readonly FunctionArgumentMode Mode = mode;
    public readonly string Name = name;
    public readonly PostgresSqlType Type = type;
}