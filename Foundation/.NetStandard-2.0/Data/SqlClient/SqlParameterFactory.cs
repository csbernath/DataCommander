using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Foundation.Assertions;
using Microsoft.SqlServer.Server;

namespace Foundation.Data.SqlClient
{
    public static class SqlParameterFactory
    {
        public static SqlParameter Create(string parameterName, SqlDbType sqlDbType, object value)
        {
            var parameter = new SqlParameter(parameterName, sqlDbType);
            parameter.Value = value;
            return parameter;
        }

        public static SqlParameter CreateNullableBit(string parameterName, bool? value)
        {
            var parameterValue = ToParameterValue(value);
            return Create(parameterName, SqlDbType.Bit, parameterValue);
        }

        public static SqlParameter CreateNullableDate(string parameterName, DateTime? value)
        {
            var parameterValue = ToParameterValue(value);
            return Create(parameterName, SqlDbType.Date, parameterValue);
        }

        public static SqlParameter CreateNullableDateTime(string parameterName, DateTime? value)
        {
            var parameterValue = ToParameterValue(value);
            return Create(parameterName, SqlDbType.DateTime, parameterValue);
        }

        public static SqlParameter CreateNullableInt(string parameterName, int? value)
        {
            var parameterValue = ToParameterValue(value);
            return Create(parameterName, SqlDbType.Int, parameterValue);
        }

        public static SqlParameter CreateChar(string parameterName, int size, string value)
        {
            var parameterValue = value != null ? (object) value : DBNull.Value;
            var parameter = new SqlParameter(parameterName, SqlDbType.Char, size);
            parameter.Value = parameterValue;
            return parameter;
        }

        public static SqlParameter CreateDate(string parameterName, DateTime value) => Create(parameterName, SqlDbType.Date, value);

        public static SqlParameter CreateNVarChar(string parameterName, int size, string value)
        {
            var parameterValue = value != null ? (object) value : DBNull.Value;
            var parameter = new SqlParameter(parameterName, SqlDbType.NVarChar, size);
            parameter.Value = parameterValue;
            return parameter;
        }

        public static SqlParameter CreateStructured(string parameterName, string typeName, IReadOnlyCollection<SqlDataRecord> sqlDataRecords)
        {
            Assert.IsNotNull(sqlDataRecords);
            var parameter = new SqlParameter
            {
                ParameterName = parameterName,
                SqlDbType = SqlDbType.Structured,
                TypeName = typeName
            };

            if (sqlDataRecords.Count > 0)
                parameter.Value = sqlDataRecords;

            return parameter;
        }

        public static SqlParameter CreateXml(string parameterName, string value) => Create(parameterName, SqlDbType.Xml, value);

        private static object ToParameterValue<T>(T? value) where T : struct => value != null ? (object) value.Value : DBNull.Value;
    }
}