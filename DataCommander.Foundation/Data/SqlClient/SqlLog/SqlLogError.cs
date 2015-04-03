namespace DataCommander.Foundation.Data
{
    using System;
    using System.Data.SqlClient;
    using System.Text;
    using DataCommander.Foundation.Data.SqlClient;

    internal sealed class SqlLogError : ISqlLogItem
    {
        private readonly int applicationId;
        private readonly int connectionNo;
        private readonly int commandNo;
        private readonly int executionNo;
        private readonly Exception exception;

        public SqlLogError(
            int applicationId,
            int connectionNo,
            int commandNo,
            int executionNo,
            Exception exception )
        {
            this.applicationId = applicationId;
            this.connectionNo = connectionNo;
            this.commandNo = commandNo;
            this.executionNo = executionNo;
            this.exception = exception;
        }

        public string CommandText
        {
            get
            {
                var sb = new StringBuilder();
                var sqlEx = this.exception as SqlException;

                if (sqlEx != null)
                {
                    SqlErrorCollection errors = sqlEx.Errors;
                    int count = errors.Count;

                    for (int i = 0; i < count; i++)
                    {
                        SqlError error = errors[ i ];
                        string procedure = error.Procedure;

                        if (procedure.Length == 0)
                        {
                            procedure = null;
                        }

                        this.AppendError( sb, i + 1, error.Number, error.Class, error.State, procedure, error.LineNumber, error.Message );
                    }
                }
                else
                {
                    string text = this.exception.ToString();
                    sb.AppendFormat( "exec LogException {0},{1},{2},{3},{4}", this.applicationId, this.connectionNo, this.commandNo, this.executionNo, text.ToTSqlVarChar() );
                }

                return sb.ToString();
            }
        }

        private void AppendError(
            StringBuilder sb,
            int errorNo,
            int error,
            byte severity,
            byte state,
            string procedure,
            int line,
            string message )
        {
            sb.AppendFormat(
                "exec LogError {0},{1},{2},{3},{4},{5},{6},{7},{8},{9}\r\n",
                this.applicationId,
                this.connectionNo,
                this.commandNo,
                this.executionNo,
                errorNo,
                error,
                severity,
                state,
                procedure.ToTSqlVarChar(),
                message.ToTSqlVarChar() );
        }
    }
}