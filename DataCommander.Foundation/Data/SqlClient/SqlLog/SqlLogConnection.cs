namespace DataCommander.Foundation.Data.SqlClient
{
    using System;
    using System.Text;
    using DataCommander.Foundation.Diagnostics;

    internal sealed class SqlLogConnection : ISqlLogItem
    {
        private readonly string name;
        private readonly string userName;
        private readonly string hostName;
        private DateTime startDate;
        private readonly long duration;
        private readonly Exception exception;

        public SqlLogConnection(
            int applicationId,
            int connectionNo,
            string name,
            string userName,
            string hostName,
            DateTime startDate,
            long duration,
            Exception exception )
        {
            this.ApplicationId = applicationId;
            this.ConnectionNo = connectionNo;
            this.name = name;
            this.userName = userName;
            this.hostName = hostName;
            this.startDate = startDate;
            this.duration = duration;
            this.exception = exception;
        }

        string ISqlLogItem.CommandText
        {
            get
            {
                var microseconds = StopwatchTimeSpan.ToInt32( this.duration, 1000000 );
                var sb = new StringBuilder();
                sb.AppendFormat(
                    "exec LogConnectionOpen {0},{1},{2},{3},{4},{5},{6}",
                    this.ApplicationId,
                    this.ConnectionNo,
                    this.name.ToTSqlVarChar(),
                    this.userName.ToTSqlVarChar(),
                    this.hostName.ToTSqlVarChar(),
                    this.startDate.ToTSqlDateTime(),
                    microseconds );

                if (this.exception != null)
                {
                    var error = new SqlLogError( this.ApplicationId, this.ConnectionNo, 0, 0, this.exception );
                    var commandText = error.CommandText;
                    sb.Append( commandText );
                }

                return sb.ToString();
            }
        }

        public int ApplicationId { get; }

        public int ConnectionNo { get; }
    }
}