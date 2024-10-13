using System;
using System.Data;
using Foundation.Assertions;

namespace Foundation.Data;

public static class DataParameterExtensions
{
    public static T? GetValueOrDefault<T>(this IDataParameter parameter) => ValueReader.GetValueOrDefault<T>(parameter.Value);

    public static void SetValue<T>(this IDataParameter parameter, DataParameterValue<T> value)
    {
        ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));
        Assert.IsInRange(value.Type is DataParameterValueType.Value or DataParameterValueType.Null or DataParameterValueType.Default);
        object? valueObject = value.Type switch
        {
            DataParameterValueType.Value => value.Value,
            DataParameterValueType.Null => DBNull.Value,
            DataParameterValueType.Default => null,
            _ => throw new ArgumentException()
        };
        parameter.Value = valueObject;
    }
}