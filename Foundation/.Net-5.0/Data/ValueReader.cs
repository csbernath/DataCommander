using System;

namespace Foundation.Data
{
    public static class ValueReader
    {
        public static TResult GetValue<TResult>(object value, object inputNullValue, TResult outputNullValue)
        {
            TResult returnValue;

            if (value == null || value == inputNullValue)
                returnValue = outputNullValue;
            else
                returnValue = (TResult)value;

            return returnValue;
        }

        public static TResult GetValue<TResult>(object value, TResult outputNullValue)
        {
            object inputNullValue = DBNull.Value;
            return GetValue(value, inputNullValue, outputNullValue);
        }

        public static TResult GetValueOrDefault<TResult>(object value)
        {
            object inputNullValue = DBNull.Value;
            var outputNullValue = default(TResult);
            return GetValue(value, inputNullValue, outputNullValue);
        }
    }
}