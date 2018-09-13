using System;
using System.Text;
using Foundation.Core;

namespace Foundation.Data.SqlClient.SqlLog
{
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
            ApplicationId = applicationId;
            ConnectionNo = connectionNo;
            _name = name;
            _userName = userName;
            _hostName = hostName;
            _startDate = startDate;
            _duration = duration;
            _exception = exception;
        }

        string ISqlLogItem.CommandText
        {
            get
            {
                var microseconds = StopwatchTimeSpan.ToInt32(_duration, 1000000 );
                var sb = new StringBuilder();
                sb.AppendFormat(
                    "exec LogConnectionOpen {0},{1},{2},{3},{4},{5},{6}",
                    ApplicationId,
                    ConnectionNo,
                    _name.ToTSqlVarChar(),
                    _userName.ToTSqlVarChar(),
                    _hostName.ToTSqlVarChar(),
                    _startDate.ToTSqlDateTime(),
                    microseconds );

                if (_exception != null)
                {
                    var error = new SqlLogError(ApplicationId, ConnectionNo, 0, 0, _exception);
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