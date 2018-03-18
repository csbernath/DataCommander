using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Foundation.Data.SqlClient
{
    public class SqlParameterCollectionBuilder
    {
        private readonly List<SqlParameter> _parameters = new List<SqlParameter>();

        public void Add(SqlParameter sqlParameter)
        {
            _parameters.Add(sqlParameter);
        }

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

        public List<object> ToObjectList()
        {
            return null;
        }
    }
}