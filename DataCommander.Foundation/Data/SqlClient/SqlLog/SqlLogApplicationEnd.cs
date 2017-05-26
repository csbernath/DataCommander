using System;

namespace Foundation.Data.SqlClient.SqlLog
{
    internal sealed class SqlLogApplicationEnd : ISqlLogItem
    {
        private readonly int _applicationId;
        private DateTime _endDate;

        public SqlLogApplicationEnd( int applicationId, DateTime endDate )
        {
            this._applicationId = applicationId;
            this._endDate = endDate;
        }

        public string CommandText => $"exec LogApplicationEnd {this._applicationId},{this._endDate.ToTSqlDateTime()}\r\n";
    }
}