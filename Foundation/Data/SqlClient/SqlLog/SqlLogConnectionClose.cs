using System;

namespace Foundation.Data.SqlClient.SqlLog
{
    internal sealed class SqlLogConnectionClose : ISqlLogItem
    {
        private readonly int _applicationId;
        private readonly int _connectionNo;
        private DateTime _endDate;

        public SqlLogConnectionClose(
            int applicationId,
            int connectionNo,
            DateTime endDate )
        {
            this._applicationId = applicationId;
            this._connectionNo = connectionNo;
            this._endDate = endDate;
        }

        public string CommandText => $"exec LogConnectionClose {this._applicationId},{this._connectionNo},{this._endDate.ToTSqlDateTime()}\r\n";
    }
}