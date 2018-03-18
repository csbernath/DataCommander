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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataRecord"></param>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        public static bool? GetNullableBoolean(this IDataRecord dataRecord, int columnIndex)
        {
            Assert.IsNotNull(dataRecord);

            return dataRecord.IsDBNull(columnIndex)
                ? (bool?)null
                : dataRecord.GetBoolean(columnIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataRecord"></param>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        public static DateTime? GetNullableDateTime(this IDataRecord dataRecord, int columnIndex)
        {
            Assert.IsNotNull(dataRecord);

            return dataRecord.IsDBNull(columnIndex)
                ? (DateTime?)null
                : dataRecord.GetDateTime(columnIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataRecord"></param>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        public static decimal? GetNullableDecimal(this IDataRecord dataRecord, int columnIndex)
        {
            Assert.IsNotNull(dataRecord);

            return dataRecord.IsDBNull(columnIndex)
                ? (decimal?)null
                : dataRecord.GetDecimal(columnIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataRecord"></param>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        public static Guid? GetNullableGuid(this IDataRecord dataRecord, int columnIndex)
        {
            Assert.IsNotNull(dataRecord);

            return dataRecord.IsDBNull(columnIndex)
                ? (Guid?) null
                : dataRecord.GetGuid(columnIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataRecord"></param>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        public static short? GetNullableInt16(this IDataRecord dataRecord, int columnIndex)
        {
            Assert.IsNotNull(dataRecord);

            return dataRecord.IsDBNull(columnIndex)
                ? (short?)null
                : dataRecord.GetInt16(columnIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataRecord"></param>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        public static int? GetNullableInt32(this IDataRecord dataRecord, int columnIndex)
        {
            Assert.IsNotNull(dataRecord);

            return dataRecord.IsDBNull(columnIndex)
                ? (int?)null
                : dataRecord.GetInt32(columnIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataRecord"></param>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        public static string GetStringOrDefault(this IDataRecord dataRecord, int columnIndex)
        {
            return dataRecord.IsDBNull(columnIndex)
                ? null
                : dataRecord.GetString(columnIndex);
        }
    }
}