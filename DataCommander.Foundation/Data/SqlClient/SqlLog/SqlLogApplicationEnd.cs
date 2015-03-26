namespace DataCommander.Foundation.Data.SqlClient
{
    using System;

    internal sealed class SqlLogApplicationEnd : ISqlLogItem
    {
        private readonly int applicationId;
        private DateTime endDate;

        public SqlLogApplicationEnd( int applicationId, DateTime endDate )
        {
            this.applicationId = applicationId;
            this.endDate = endDate;
        }

        public string CommandText
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