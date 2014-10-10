namespace DataCommander.Foundation.Data
{
    using System;
    using System.Data.SqlClient;
    using System.Text;
    using DataCommander.Foundation.Data.SqlClient;

    internal sealed class SqlLogError : ISqlLogItem
    {
        private readonly Int32 applicationId;
        private Int32 connectionNo;
        private Int32 commandNo;
        private Int32 executionNo;
        private Exception exception;

        public SqlLogError(
            Int32 applicationId,
            Int32 connectionNo,
            Int32 commandNo,
            Int32 executionNo,
            Exception exception )
        {
            this.applicationId = applicationId;
            this.connectionNo = connectionNo;
            this.commandNo = commandNo;
            this.executionNo = executionNo;
            this.exception = exception;
        }

        public String CommandText
        {
            get
            {
                var sb = new StringBuilder();
                var sqlEx = this.exception as SqlException;

                if (sqlEx != null)
                {
                    SqlErrorCollection errors = sqlEx.Errors;
                    Int32 count = errors.Count;

                    for (Int32 i = 0; i < count; i++)
                    {
                        SqlError error = errors[ i ];
                        String procedure = error.Procedure;

                        if (procedure.Length == 0)
                        {
                            procedure = null;
                        }

                        this.AppendError( sb, i + 1, error.Number, error.Class, error.State, procedure, error.LineNumber, error.Message );
                    }
                }
                else
                {
                    String text = this.exception.ToString();
                    sb.AppendFormat( "exec LogException {0},{1},{2},{3},{4}", this.applicationId, this.connectionNo, this.commandNo, this.executionNo, text.ToTSqlVarChar() );
                }

                return sb.ToString();
            }
        }

        private void AppendError(
            StringBuilder sb,
            Int32 errorNo,
            Int32 error,
            Byte severity,
            Byte state,
            String procedure,
            Int32 line,
            String message )
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