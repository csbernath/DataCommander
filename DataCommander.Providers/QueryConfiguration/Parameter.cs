namespace DataCommander.Providers.QueryConfiguration
{
    public class Parameter
    {
        public readonly string Name;
        public readonly string DataType;
        public readonly string Value;

        public Parameter(string name, string dataType, string value)
        {
            Name = name;
            DataType = dataType;
            Value = value;
        }
    }
}