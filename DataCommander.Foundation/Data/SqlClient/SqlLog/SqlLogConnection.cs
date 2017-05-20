namespace DataCommander.Foundation.Data.SqlClient.SqlLog
{
    using System;
    using System.Text;
    using DataCommander.Foundation.Diagnostics;

    internal sealed class SqlLogConnection : ISqlLogItem
    {
        private readonly string _name;
        private readonly string _userName;
        private readonly string _hostName;
        private DateTime _startDate;
        private readonly long _duration;
        private readonly Exception _exception;

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
            this._name = name;
            this._userName = userName;
            this._hostName = hostName;
            this._startDate = startDate;
            this._duration = duration;
            this._exception = exception;
        }

        string ISqlLogItem.CommandText
        {
            get
            {
                var microseconds = StopwatchTimeSpan.ToInt32( this._duration, 1000000 );
                var sb = new StringBuilder();
                sb.AppendFormat(
                    "exec LogConnectionOpen {0},{1},{2},{3},{4},{5},{6}",
                    this.ApplicationId,
                    this.ConnectionNo,
                    this._name.ToTSqlVarChar(),
                    this._userName.ToTSqlVarChar(),
                    this._hostName.ToTSqlVarChar(),
                    this._startDate.ToTSqlDateTime(),
                    microseconds );

                if (this._exception != null)
                {
                    var error = new SqlLogError( this.ApplicationId, this.ConnectionNo, 0, 0, this._exception );
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