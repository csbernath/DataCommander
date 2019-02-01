using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Foundation.Collections.ReadOnly;
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
            var parameter = new SqlParameter(parameterName, value);
            Add(parameter);
        }

        public void Add(string parameterName, SqlDbType sqlDbType, object value)
        {
            var parameter = SqlParameterFactory.Create(parameterName, sqlDbType, value);
            Add(parameter);
        }

        public void AddDate(string parameterName, DateTime value)
        {
            var parameter = SqlParameterFactory.CreateDate(parameterName, value);
            Add(parameter);
        }

        public void AddNVarChar(string parameterName, int size, string value)
        {
            var parameter = SqlParameterFactory.CreateNVarChar(parameterName, size, value);
            Add(parameter);
        }

        public void AddStructured(string parameterName, string typeName, ReadOnlyList<SqlDataRecord> sqlDataRecords)
        {
            var parameter = SqlParameterFactory.CreateStructured(parameterName, typeName, sqlDataRecords);
            Add(parameter);
        }

        public void AddXml(string parameterName, string value)
        {
            var parameter = SqlParameterFactory.CreateXml(parameterName, value);
            Add(parameter);
        }

        public ReadOnlyList<object> ToReadOnlyList() => _parameters.Cast<object>().ToReadOnlyList();
    }
}