using System;

namespace Foundation.Data.SqlClient.SqlLog;

internal sealed class SqlLogApplicationEnd(int applicationId, DateTime endDate) : ISqlLogItem
{
    public string CommandText => $"exec LogApplicationEnd {applicationId},{endDate.ToSqlConstant()}\r\n";
}