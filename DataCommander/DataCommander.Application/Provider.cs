namespace DataCommander.Application;

public class Provider
{
    public readonly string Identifier;
    public readonly string Name;

    public Provider(string identifier, string name)
    {
        Identifier = identifier;
        Name = name;
    }
}