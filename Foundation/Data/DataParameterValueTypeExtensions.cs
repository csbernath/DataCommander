
namespace Foundation.Data;

public static class DataParameterValueTypeExtensions
{
    extension(DataParameterValueType type)
    {
        public bool IsValueOrNull() => type == DataParameterValueType.Value || type == DataParameterValueType.Null;
    }
}