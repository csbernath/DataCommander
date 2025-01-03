﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using Foundation.Collections.ReadOnly;
using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient.Server;

namespace Foundation.Data.SqlClient;

public class SqlParameterCollectionBuilder
{
    private readonly List<SqlParameter> _parameters = [];

    public void Add(SqlParameter sqlParameter) => _parameters.Add(sqlParameter);

    public void Add(string parameterName, object value)
    {
        var parameter = new SqlParameter(parameterName, value);
        Add(parameter);
    }

    public void AddNullableBit(string parameterName, bool? value)
    {
        var parameter = SqlParameterFactory.CreateNullableBit(parameterName, value);
        Add(parameter);
    }

    public void AddNullableDate(string parameterName, DateTime? value)
    {
        var parameter = SqlParameterFactory.CreateNullableDate(parameterName, value);
        Add(parameter);
    }

    public void AddNullableDateTime(string parameterName, DateTime? value)
    {
        var parameter = SqlParameterFactory.CreateNullableDateTime(parameterName, value);
        Add(parameter);
    }

    public void AddNullableGuid(string parameterName, Guid? value)
    {
        var parameter = SqlParameterFactory.CreateNullableGuid(parameterName, value);
        Add(parameter);
    }

    public void AddNullableInt(string parameterName, int? value)
    {
        var parameter = SqlParameterFactory.CreateNullableInt(parameterName, value);
        Add(parameter);
    }

    public void Add(string parameterName, SqlDbType sqlDbType, object value)
    {
        var parameter = SqlParameterFactory.Create(parameterName, sqlDbType, value);
        Add(parameter);
    }

    public void AddChar(string parameterName, int size, string value)
    {
        var parameter = SqlParameterFactory.CreateChar(parameterName, size, value);
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

    public void AddStructured(string parameterName, string typeName, IReadOnlyCollection<SqlDataRecord> sqlDataRecords)
    {
        var parameter = SqlParameterFactory.CreateStructured(parameterName, typeName, sqlDataRecords);
        Add(parameter);
    }

    public void AddVarChar(string parameterName, int size, string value)
    {
        var parameter = SqlParameterFactory.CreateVarChar(parameterName, size, value);
        Add(parameter);
    }

    public void AddXml(string parameterName, string value)
    {
        var parameter = SqlParameterFactory.CreateXml(parameterName, value);
        Add(parameter);
    }

    public ReadOnlyCollection<object> ToReadOnlyCollection() => _parameters.Cast<object>().ToReadOnlyCollection();
}