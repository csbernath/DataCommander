using System;
using System.Data;
using System.Data.SqlClient;
using Foundation.Assertions;
using Foundation.Collections.ReadOnly;
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

        public static SqlParameter CreateDate(string parameterName, DateTime value) => Create(parameterName, SqlDbType.Date, value);

        public static SqlParameter CreateString(string parameterName, string value)
        {
            var parameterValue = value != null ? (object) value : DBNull.Value;
            var parameter = new SqlParameter(parameterName, parameterValue);
            return parameter;
        }

        public static SqlParameter CreateStructured(string parameterName, string typeName, ReadOnlyList<SqlDataRecord> sqlDataRecords)
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
    }
}