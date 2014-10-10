namespace DataCommander.Foundation.Data
{
    using System;
    using DataCommander.Foundation.Data.SqlClient;

    internal sealed class SqlLogConnectionClose : ISqlLogItem
    {
        private readonly Int32 applicationId;
        private readonly Int32 connectionNo;
        private DateTime endDate;

        public SqlLogConnectionClose(
            Int32 applicationId,
            Int32 connectionNo,
            DateTime endDate )
        {
            this.applicationId = applicationId;
            this.connectionNo = connectionNo;
            this.endDate = endDate;
        }

        public String CommandText
        {
            get
            {
                return string.Format( "exec LogConnectionClose {0},{1},{2}\r\n", this.applicationId, this.connectionNo, this.endDate.ToTSqlDateTime() );
            }
        }
    }
}