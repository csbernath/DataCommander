using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using Foundation.Assertions;
using Microsoft.SqlServer.Server;

namespace Foundation.Data.SqlClient
{
    public static class SqlParameterFactory
    {
        public static SqlParameter CreateStructured(string parameterName, string typeName, ReadOnlyCollection<SqlDataRecord> sqlDataRecords)
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
    }
}