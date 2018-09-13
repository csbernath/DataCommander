namespace Foundation.Data
{
    public static class DataParameterValueTypeExtensions
    {
        public static bool IsValueOrNull(this DataParameterValueType type)
        {
            return type == DataParameterValueType.Value || type == DataParameterValueType.Null;
        }
    }
}