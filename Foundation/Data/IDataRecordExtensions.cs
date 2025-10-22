using System;
using System.Data;

namespace Foundation.Data;

public static class IDataRecordExtensions
{
    extension(IDataRecord dataRecord)
    {
        public byte[] GetBytes(int fieldIndex)
        {
            ArgumentNullException.ThrowIfNull(dataRecord);
            var valueObject = dataRecord.GetValue(fieldIndex);
            var value = (byte[])valueObject;
            return value;
        }

        public bool? GetNullableBoolean(int fieldIndex)
        {
            ArgumentNullException.ThrowIfNull(dataRecord);
            return dataRecord.IsDBNull(fieldIndex)
                ? (bool?)null
                : dataRecord.GetBoolean(fieldIndex);
        }

        public byte? GetNullableByte(int fieldIndex)
        {
            ArgumentNullException.ThrowIfNull(dataRecord);
            return dataRecord.IsDBNull(fieldIndex)
                ? null
                : dataRecord.GetByte(fieldIndex);
        }

        public char[]? GetNullableCharArray(int fieldIndex)
        {
            ArgumentNullException.ThrowIfNull(dataRecord);
            return dataRecord.IsDBNull(fieldIndex)
                ? null
                : (char[])dataRecord.GetValue(fieldIndex);
        }

        public DateTime? GetNullableDateTime(int fieldIndex)
        {
            ArgumentNullException.ThrowIfNull(dataRecord);
            return dataRecord.IsDBNull(fieldIndex)
                ? (DateTime?)null
                : dataRecord.GetDateTime(fieldIndex);
        }

        public decimal? GetNullableDecimal(int fieldIndex)
        {
            ArgumentNullException.ThrowIfNull(dataRecord);
            return dataRecord.IsDBNull(fieldIndex)
                ? (decimal?)null
                : dataRecord.GetDecimal(fieldIndex);
        }

        public double? GetNullableDouble(int fieldIndex)
        {
            ArgumentNullException.ThrowIfNull(dataRecord);
            return dataRecord.IsDBNull(fieldIndex)
                ? (double?)null
                : dataRecord.GetDouble(fieldIndex);
        }

        public Guid? GetNullableGuid(int fieldIndex)
        {
            ArgumentNullException.ThrowIfNull(dataRecord);
            return dataRecord.IsDBNull(fieldIndex)
                ? (Guid?)null
                : dataRecord.GetGuid(fieldIndex);
        }

        public short? GetNullableInt16(int fieldIndex)
        {
            ArgumentNullException.ThrowIfNull(dataRecord);
            return dataRecord.IsDBNull(fieldIndex)
                ? (short?)null
                : dataRecord.GetInt16(fieldIndex);
        }

        public int? GetNullableInt32(int fieldIndex)
        {
            ArgumentNullException.ThrowIfNull(dataRecord);
            return dataRecord.IsDBNull(fieldIndex)
                ? (int?)null
                : dataRecord.GetInt32(fieldIndex);
        }

        public long? GetNullableInt64(int fieldIndex)
        {
            ArgumentNullException.ThrowIfNull(dataRecord);
            return dataRecord.IsDBNull(fieldIndex)
                ? (long?)null
                : dataRecord.GetInt64(fieldIndex);
        }

        public string[]? GetNullableStringArray(int fieldIndex)
        {
            ArgumentNullException.ThrowIfNull(dataRecord);
            return dataRecord.IsDBNull(fieldIndex)
                ? null
                : (string[])dataRecord.GetValue(fieldIndex);
        }

        [CLSCompliant(false)]
        public uint[]? GetNullableUInt32Array(int fieldIndex)
        {
            ArgumentNullException.ThrowIfNull(dataRecord);
            return dataRecord.IsDBNull(fieldIndex)
                ? null
                : (uint[])dataRecord.GetValue(fieldIndex);
        }

        public string? GetStringOrDefault(int fieldIndex)
        {
            ArgumentNullException.ThrowIfNull(dataRecord);
            return dataRecord.IsDBNull(fieldIndex)
                ? null
                : dataRecord.GetString(fieldIndex);
        }

        [CLSCompliant(false)]
        public uint GetUInt32(int fieldIndex) => (uint)dataRecord.GetValue(fieldIndex);
    }
}