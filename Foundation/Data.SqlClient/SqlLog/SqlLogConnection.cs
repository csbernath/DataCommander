using System;
using System.Text;
using Foundation.Core;

namespace Foundation.Data.SqlClient.SqlLog;

internal sealed class SqlLogConnection(
    int applicationId,
    int connectionNo,
    string name,
    string userName,
    string hostName,
    DateTime startDate,
    long duration,
    Exception? exception)
    : ISqlLogItem
{
    string ISqlLogItem.CommandText
    {
        get
        {
            var microseconds = StopwatchTimeSpan.ToInt32(duration, 1000000);
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(
                $"exec LogConnectionOpen {ApplicationId},{ConnectionNo},{name.ToNullableVarChar()},{userName.ToNullableVarChar()},{hostName.ToNullableVarChar()},{startDate.ToSqlConstant()},{microseconds}");

            if (exception != null)
            {
                var error = new SqlLogError(ApplicationId, ConnectionNo, 0, 0, exception);
                var commandText = error.CommandText;
                stringBuilder.Append(commandText);
            }

            return stringBuilder.ToString();
        }
    }

    public int ApplicationId { get; } = applicationId;

    public int ConnectionNo { get; } = connectionNo;
}