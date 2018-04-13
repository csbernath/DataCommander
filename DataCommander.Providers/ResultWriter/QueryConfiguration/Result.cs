namespace DataCommander.Providers.ResultWriter.QueryConfiguration
{
    public class Result
    {
        public readonly string Name;
        public readonly string FieldName;

        public Result(string name, string fieldName)
        {
            Name = name;
            FieldName = fieldName;
        }
    }
}