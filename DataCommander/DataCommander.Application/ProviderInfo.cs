namespace DataCommander.Application;

public sealed class ProviderInfo(string identifier, string name)
{
    public readonly string Identifier = identifier;
    public readonly string Name = name;
}