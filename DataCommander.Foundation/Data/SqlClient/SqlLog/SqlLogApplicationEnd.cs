namespace DataCommander.Foundation.Data.SqlClient
{
    using System;

    internal sealed class SqlLogApplicationEnd : ISqlLogItem
    {
        private readonly Int32 applicationId;
        private DateTime endDate;

        public SqlLogApplicationEnd( Int32 applicationId, DateTime endDate )
        {
            this.applicationId = applicationId;
            this.endDate = endDate;
        }

        public String CommandText
        {
            get
            {
                return string.Format(
                    "exec LogApplicationEnd {0},{1}\r\n",
                    this.applicationId,
                    this.endDate.ToTSqlDateTime() );
            }
        }
    }
}