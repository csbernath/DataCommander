using System;
using DataCommander.Api;
using Microsoft.Data.SqlClient;

namespace DataCommander.Providers.SqlServer;

internal sealed class SqlDataParameter : DataParameterBase
{
    private readonly SqlParameter _parameter;

    public SqlDataParameter(SqlParameter parameter)
        : base(parameter, parameter.Size, parameter.Precision, parameter.Scale)
    {
        ArgumentNullException.ThrowIfNull(parameter);
        _parameter = parameter;
    }

    protected override void SetSize(int size) => _parameter.Size = size;
}