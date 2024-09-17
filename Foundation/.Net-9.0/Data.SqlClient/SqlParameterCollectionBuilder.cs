using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using Foundation.Collections.ReadOnly;
using Foundation.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient.Server;

namespace Foundation.Data.SqlClient;

public class SqlParameterCollectionBuilder
{
    private readonly List<SqlParameter> _parameters = [];

    public void Add(SqlParameter sqlParameter) => _parameters.Add(sqlParameter);

    public void Add(string parameterName, object value)
    {
        SqlParameter parameter = new SqlParameter(parameterName, value);
        Add(parameter);
    }

    public void AddNullableBit(string parameterName, bool? value)
    {
        SqlParameter parameter = SqlParameterFactory.CreateNullableBit(parameterName, value);
        Add(parameter);
    }

    public void AddNullableDate(string parameterName, DateTime? value)
    {
        SqlParameter parameter = SqlParameterFactory.CreateNullableDate(parameterName, value);
        Add(parameter);
    }

    public void AddNullableDateTime(string parameterName, DateTime? value)
    {
        SqlParameter parameter = SqlParameterFactory.CreateNullableDateTime(parameterName, value);
        Add(parameter);
    }

    public void AddNullableGuid(string parameterName, Guid? value)
    {
        SqlParameter parameter = SqlParameterFactory.CreateNullableGuid(parameterName, value);
        Add(parameter);
    }

    public void AddNullableInt(string parameterName, int? value)
    {
        SqlParameter parameter = SqlParameterFactory.CreateNullableInt(parameterName, value);
        Add(parameter);
    }

    public void Add(string parameterName, SqlDbType sqlDbType, object value)
    {
        SqlParameter parameter = SqlParameterFactory.Create(parameterName, sqlDbType, value);
        Add(parameter);
    }

    public void AddChar(string parameterName, int size, string value)
    {
        SqlParameter parameter = SqlParameterFactory.CreateChar(parameterName, size, value);
        Add(parameter);
    }

    public void AddDate(string parameterName, DateTime value)
    {
        SqlParameter parameter = SqlParameterFactory.CreateDate(parameterName, value);
        Add(parameter);
    }

    public void AddNVarChar(string parameterName, int size, string value)
    {
        SqlParameter parameter = SqlParameterFactory.CreateNVarChar(parameterName, size, value);
        Add(parameter);
    }

    public void AddStructured(string parameterName, string typeName, IReadOnlyCollection<SqlDataRecord> sqlDataRecords)
    {
        SqlParameter parameter = SqlParameterFactory.CreateStructured(parameterName, typeName, sqlDataRecords);
        Add(parameter);
    }

    public void AddVarChar(string parameterName, int size, string value)
    {
        SqlParameter parameter = SqlParameterFactory.CreateVarChar(parameterName, size, value);
        Add(parameter);
    }

    public void AddXml(string parameterName, string value)
    {
        SqlParameter parameter = SqlParameterFactory.CreateXml(parameterName, value);
        Add(parameter);
    }

    public ReadOnlyCollection<object> ToReadOnlyCollection() => _parameters.Cast<object>().ToReadOnlyCollection();
}