using System;
using System.Data;
using Foundation.Assertions;

namespace Foundation.Data;

public static class DataParameterExtensions
{
    extension(IDataParameter parameter)
    {
        public T? GetValueOrDefault<T>() => ValueReader.GetValueOrDefault<T>(parameter.Value);

        public void SetValue<T>(DataParameterValue<T> value)
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
}