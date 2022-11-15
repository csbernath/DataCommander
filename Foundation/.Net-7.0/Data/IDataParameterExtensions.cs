using System;
using System.Data;
using Foundation.Assertions;

namespace Foundation.Data;

public static class DataParameterExtensions
{
    public static T GetValueOrDefault<T>(this IDataParameter parameter) => ValueReader.GetValueOrDefault<T>(parameter.Value);

    public static void SetValue<T>(this IDataParameter parameter, DataParameterValue<T> value)
    {
        ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));
        Assert.IsInRange(value.Type == DataParameterValueType.Value || value.Type == DataParameterValueType.Null ||
                         value.Type == DataParameterValueType.Default);

        object valueObject;

        switch (value.Type)
        {
            case DataParameterValueType.Value:
                valueObject = value.Value;
                break;

            case DataParameterValueType.Null:
                valueObject = DBNull.Value;
                break;

            case DataParameterValueType.Default:
                valueObject = null;
                break;

            default:
                throw new ArgumentException();
        }

        parameter.Value = valueObject;
    }
}