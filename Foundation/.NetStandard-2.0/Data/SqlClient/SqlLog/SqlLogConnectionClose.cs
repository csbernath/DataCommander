using System;

namespace Foundation.Data.SqlClient.SqlLog
{
    internal sealed class SqlLogConnectionClose : ISqlLogItem
    {
        private readonly int _applicationId;
        private readonly int _connectionNo;
        private readonly DateTime _endDate;

        public SqlLogConnectionClose(
            int applicationId,
            int connectionNo,
            DateTime endDate )
        {
            _applicationId = applicationId;
            _connectionNo = connectionNo;
            _endDate = endDate;
        }

        public string CommandText => $"exec LogConnectionClose {_applicationId},{_connectionNo},{_endDate.ToSqlConstant()}\r\n";
    }
}