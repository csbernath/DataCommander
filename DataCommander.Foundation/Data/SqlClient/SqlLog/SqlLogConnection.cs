namespace DataCommander.Foundation.Data.SqlClient
{
    using System;
    using System.Text;
    using DataCommander.Foundation.Diagnostics;

    internal sealed class SqlLogConnection : ISqlLogItem
    {
        private readonly Int32 applicationId;
        private readonly Int32 connectionNo;
        private readonly String name;
        private String userName;
        private String hostName;
        private DateTime startDate;
        private Int64 duration;
        private Exception exception;

        public SqlLogConnection(
            Int32 applicationId,
            Int32 connectionNo,
            String name,
            String userName,
            String hostName,
            DateTime startDate,
            Int64 duration,
            Exception exception )
        {
            this.applicationId = applicationId;
            this.connectionNo = connectionNo;
            this.name = name;
            this.userName = userName;
            this.hostName = hostName;
            this.startDate = startDate;
            this.duration = duration;
            this.exception = exception;
        }

        String ISqlLogItem.CommandText
        {
            get
            {
                Int32 microseconds = StopwatchTimeSpan.ToInt32( this.duration, 1000000 );
                var sb = new StringBuilder();
                sb.AppendFormat(
                    "exec LogConnectionOpen {0},{1},{2},{3},{4},{5},{6}",
                    this.applicationId,
                    this.connectionNo,
                    this.name.ToTSqlVarChar(),
                    this.userName.ToTSqlVarChar(),
                    this.hostName.ToTSqlVarChar(),
                    this.startDate.ToTSqlDateTime(),
                    microseconds );

                if (this.exception != null)
                {
                    SqlLogError error = new SqlLogError( this.applicationId, this.connectionNo, 0, 0, this.exception );
                    String commandText = error.CommandText;
                    sb.Append( commandText );
                }

                return sb.ToString();
            }
        }

        public Int32 ApplicationId
        {
            get
            {
                return this.applicationId;
            }
        }

        public Int32 ConnectionNo
        {
            get
            {
                return this.connectionNo;
            }
        }
    }
}