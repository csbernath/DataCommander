using System;
using System.Data;

namespace Foundation.Data;

public static class IDataRecordExtensions
{
    public static byte[] GetBytes(this IDataRecord dataRecord, int fieldIndex)
    {
        ArgumentNullException.ThrowIfNull(dataRecord);
        object valueObject = dataRecord.GetValue(fieldIndex);
        byte[] value = (byte[])valueObject;
        return value;
    }

    public static bool? GetNullableBoolean(this IDataRecord dataRecord, int fieldIndex)
    {
        ArgumentNullException.ThrowIfNull(dataRecord);
        return dataRecord.IsDBNull(fieldIndex)
            ? (bool?)null
            : dataRecord.GetBoolean(fieldIndex);
    }

    public static byte? GetNullableByte(this IDataRecord dataRecord, int fieldIndex)
    {
        ArgumentNullException.ThrowIfNull(dataRecord);
        return dataRecord.IsDBNull(fieldIndex)
            ? (byte?)null
            : dataRecord.GetByte(fieldIndex);
    }

    public static DateTime? GetNullableDateTime(this IDataRecord dataRecord, int fieldIndex)
    {
        ArgumentNullException.ThrowIfNull(dataRecord);
        return dataRecord.IsDBNull(fieldIndex)
            ? (DateTime?)null
            : dataRecord.GetDateTime(fieldIndex);
    }

    public static decimal? GetNullableDecimal(this IDataRecord dataRecord, int fieldIndex)
    {
        ArgumentNullException.ThrowIfNull(dataRecord);
        return dataRecord.IsDBNull(fieldIndex)
            ? (decimal?)null
            : dataRecord.GetDecimal(fieldIndex);
    }

    public static double? GetNullableDouble(this IDataRecord dataRecord, int fieldIndex)
    {
        ArgumentNullException.ThrowIfNull(dataRecord);
        return dataRecord.IsDBNull(fieldIndex)
            ? (double?)null
            : dataRecord.GetDouble(fieldIndex);
    }

    public static Guid? GetNullableGuid(this IDataRecord dataRecord, int fieldIndex)
    {
        ArgumentNullException.ThrowIfNull(dataRecord);
        return dataRecord.IsDBNull(fieldIndex)
            ? (Guid?)null
            : dataRecord.GetGuid(fieldIndex);
    }

    public static short? GetNullableInt16(this IDataRecord dataRecord, int fieldIndex)
    {
        ArgumentNullException.ThrowIfNull(dataRecord);
        return dataRecord.IsDBNull(fieldIndex)
            ? (short?)null
            : dataRecord.GetInt16(fieldIndex);
    }

    public static int? GetNullableInt32(this IDataRecord dataRecord, int fieldIndex)
    {
        ArgumentNullException.ThrowIfNull(dataRecord);
        return dataRecord.IsDBNull(fieldIndex)
            ? (int?)null
            : dataRecord.GetInt32(fieldIndex);
    }

    public static long? GetNullableInt64(this IDataRecord dataRecord, int fieldIndex)
    {
        ArgumentNullException.ThrowIfNull(dataRecord);
        return dataRecord.IsDBNull(fieldIndex)
            ? (long?)null
            : dataRecord.GetInt64(fieldIndex);
    }

    public static string GetStringOrDefault(this IDataRecord dataRecord, int fieldIndex)
    {
        ArgumentNullException.ThrowIfNull(dataRecord);
        return dataRecord.IsDBNull(fieldIndex)
            ? null
            : dataRecord.GetString(fieldIndex);
    }
}