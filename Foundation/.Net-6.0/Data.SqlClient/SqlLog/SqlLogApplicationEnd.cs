﻿using System;

namespace Foundation.Data.SqlClient.SqlLog;

internal sealed class SqlLogApplicationEnd : ISqlLogItem
{
    private readonly int _applicationId;
    private readonly DateTime _endDate;

    public SqlLogApplicationEnd(int applicationId, DateTime endDate)
    {
        _applicationId = applicationId;
        _endDate = endDate;
    }

    public string CommandText => $"exec LogApplicationEnd {_applicationId},{_endDate.ToSqlConstant()}\r\n";
}