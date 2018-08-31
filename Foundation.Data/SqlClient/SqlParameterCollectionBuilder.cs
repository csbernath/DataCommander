using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Foundation.Collections;
using Foundation.Linq;
using Microsoft.SqlServer.Server;

namespace Foundation.Data.SqlClient
{
    public class SqlParameterCollectionBuilder
    {
        private readonly List<SqlParameter> _parameters = new List<SqlParameter>();

        public void Add(SqlParameter sqlParameter) => _parameters.Add(sqlParameter);

        public void Add(string parameterName, object value)
        {
            var parameter = new SqlParameter
            {
                ParameterName = parameterName,
                Value = value
            };
            Add(parameter);
        }

        public void Add(string parameterName, SqlDbType sqlDbType, object value)
        {
            var parameter = new SqlParameter
            {
                ParameterName = parameterName,
                SqlDbType = sqlDbType,
                Value = value
            };
            Add(parameter);
        }

        public void AddDate(string parameterName, DateTime value)
        {
            var parameter = new SqlParameter
            {
                ParameterName = parameterName,
                SqlDbType = SqlDbType.Date,
                Value = value
            };
            Add(parameter);
        }

        public void AddStructured(string parameterName, string typeName, ReadOnlyList<SqlDataRecord> sqlDataRecords)
        {
            var parameter = SqlParameterFactory.CreateStructured(parameterName, typeName, sqlDataRecords);
            Add(parameter);
        }

        public ReadOnlyList<object> ToReadOnlyList() => _parameters.Cast<object>().ToReadOnlyList();
    }
}