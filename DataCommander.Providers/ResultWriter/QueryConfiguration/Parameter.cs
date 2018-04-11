namespace DataCommander.Providers.ResultWriter.QueryConfiguration
{
    public class Parameter
    {
        public readonly string Name;
        public readonly string DataType;
        public readonly bool IsNullable;
        public readonly string Value;

        public Parameter(string name, string dataType, bool isNullable, string value)
        {
            Name = name;
            DataType = dataType;
            IsNullable = isNullable;
            Value = value;
        }
    }
}