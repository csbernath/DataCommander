using System;
using System.Data;
using Foundation.Diagnostics.Assertions;

namespace Foundation.Data
{
    /// <summary>
    /// 
    /// </summary>
    public static class IDataRecordExtensions
    {
        public static byte[] GetBytes(this IDataRecord dataRecord, int fieldIndex)
        {
            var valueObject = dataRecord.GetValue(fieldIndex);
            var value = (byte[]) valueObject;
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataRecord"></param>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        public static bool? GetNullableBoolean(this IDataRecord dataRecord, int fieldIndex)
        {
            Assert.IsNotNull(dataRecord);

            return dataRecord.IsDBNull(fieldIndex)
                ? (bool?) null
                : dataRecord.GetBoolean(fieldIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataRecord"></param>
        /// <param name="fieldIndex"></param>
        /// <returns></returns>
        public static DateTime? GetNullableDateTime(this IDataRecord dataRecord, int fieldIndex)
        {
            Assert.IsNotNull(dataRecord);

            return dataRecord.IsDBNull(fieldIndex)
                ? (DateTime?) null
                : dataRecord.GetDateTime(fieldIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataRecord"></param>
        /// <param name="fieldIndex"></param>
        /// <returns></returns>
        public static decimal? GetNullableDecimal(this IDataRecord dataRecord, int fieldIndex)
        {
            Assert.IsNotNull(dataRecord);

            return dataRecord.IsDBNull(fieldIndex)
                ? (decimal?) null
                : dataRecord.GetDecimal(fieldIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataRecord"></param>
        /// <param name="fieldIndex"></param>
        /// <returns></returns>
        public static Guid? GetNullableGuid(this IDataRecord dataRecord, int fieldIndex)
        {
            Assert.IsNotNull(dataRecord);

            return dataRecord.IsDBNull(fieldIndex)
                ? (Guid?) null
                : dataRecord.GetGuid(fieldIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataRecord"></param>
        /// <param name="fieldIndex"></param>
        /// <returns></returns>
        public static short? GetNullableInt16(this IDataRecord dataRecord, int fieldIndex)
        {
            Assert.IsNotNull(dataRecord);

            return dataRecord.IsDBNull(fieldIndex)
                ? (short?) null
                : dataRecord.GetInt16(fieldIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataRecord"></param>
        /// <param name="fieldIndex"></param>
        /// <returns></returns>
        public static int? GetNullableInt32(this IDataRecord dataRecord, int fieldIndex)
        {
            Assert.IsNotNull(dataRecord);

            return dataRecord.IsDBNull(fieldIndex)
                ? (int?) null
                : dataRecord.GetInt32(fieldIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataRecord"></param>
        /// <param name="fieldIndex"></param>
        /// <returns></returns>
        public static string GetStringOrDefault(this IDataRecord dataRecord, int fieldIndex)
        {
            return dataRecord.IsDBNull(fieldIndex)
                ? null
                : dataRecord.GetString(fieldIndex);
        }
    }
}