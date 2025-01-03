namespace Foundation.Collections;

public sealed class CommandLineArgument(int index, string name, string value)
{
    public readonly int Index = index;
    public readonly string Name = name;
    public readonly string Value = value;
}