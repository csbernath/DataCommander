namespace DataCommander.Foundation.Data.SqlClient.SqlLog
{
    using System;

    internal sealed class SqlLogConnectionClose : ISqlLogItem
    {
        private readonly int applicationId;
        private readonly int connectionNo;
        private DateTime endDate;

        public SqlLogConnectionClose(
            int applicationId,
            int connectionNo,
            DateTime endDate )
        {
            this.applicationId = applicationId;
            this.connectionNo = connectionNo;
            this.endDate = endDate;
        }

        public string CommandText => $"exec LogConnectionClose {this.applicationId},{this.connectionNo},{this.endDate.ToTSqlDateTime()}\r\n";
    }
}