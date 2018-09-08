using System;
using System.Data;
using Foundation.Assertions;

namespace Foundation.Data
{
    public static class DataRecordExtensions
    {
        public static byte[] GetBytes(this IDataRecord dataRecord, int fieldIndex)
        {
            var valueObject = dataRecord.GetValue(fieldIndex);
            var value = (byte[]) valueObject;
            return value;
        }

        public static bool? GetNullableBoolean(this IDataRecord dataRecord, int fieldIndex)
        {
            Assert.IsNotNull(dataRecord);

            return dataRecord.IsDBNull(fieldIndex)
                ? (bool?) null
                : dataRecord.GetBoolean(fieldIndex);
        }

        public static DateTime? GetNullableDateTime(this IDataRecord dataRecord, int fieldIndex)
        {
            Assert.IsNotNull(dataRecord);

            return dataRecord.IsDBNull(fieldIndex)
                ? (DateTime?) null
                : dataRecord.GetDateTime(fieldIndex);
        }

        public static decimal? GetNullableDecimal(this IDataRecord dataRecord, int fieldIndex)
        {
            Assert.IsNotNull(dataRecord);

            return dataRecord.IsDBNull(fieldIndex)
                ? (decimal?) null
                : dataRecord.GetDecimal(fieldIndex);
        }

        public static Guid? GetNullableGuid(this IDataRecord dataRecord, int fieldIndex)
        {
            Assert.IsNotNull(dataRecord);

            return dataRecord.IsDBNull(fieldIndex)
                ? (Guid?) null
                : dataRecord.GetGuid(fieldIndex);
        }

        public static short? GetNullableInt16(this IDataRecord dataRecord, int fieldIndex)
        {
            Assert.IsNotNull(dataRecord);

            return dataRecord.IsDBNull(fieldIndex)
                ? (short?) null
                : dataRecord.GetInt16(fieldIndex);
        }

        public static int? GetNullableInt32(this IDataRecord dataRecord, int fieldIndex)
        {
            Assert.IsNotNull(dataRecord);

            return dataRecord.IsDBNull(fieldIndex)
                ? (int?) null
                : dataRecord.GetInt32(fieldIndex);
        }

        public static string GetStringOrDefault(this IDataRecord dataRecord, int fieldIndex)
        {
            return dataRecord.IsDBNull(fieldIndex)
                ? null
                : dataRecord.GetString(fieldIndex);
        }
    }
}