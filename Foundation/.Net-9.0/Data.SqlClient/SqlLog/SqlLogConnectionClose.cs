using System;

namespace Foundation.Data.SqlClient.SqlLog;

internal sealed class SqlLogConnectionClose(
    int applicationId,
    int connectionNo,
    DateTime endDate) : ISqlLogItem
{
    public string CommandText => $"exec LogConnectionClose {applicationId},{connectionNo},{endDate.ToSqlConstant()}\r\n";
}