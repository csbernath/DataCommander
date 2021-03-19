namespace Foundation.Collections
{
    public sealed class CommandLineArgument
    {
        public readonly int Index;
        public readonly string Name;
        public readonly string Value;

        public CommandLineArgument(int index, string name, string value)
        {
            Index = index;
            Name = name;
            Value = value;
        }
    }
}