namespace DataCommander.Foundation.Data.SqlClient
{
    using System;
    using System.Data.SqlClient;
    using System.Diagnostics.Contracts;
    using System.Text;

    /// <summary>
    /// 
    /// </summary>
    public static class SqlErrorExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public static string ToLogString(this SqlError error)
        {
            Contract.Requires<ArgumentNullException>(error != null);

            var sb = new StringBuilder();

            bool hasProcedure = !string.IsNullOrEmpty(error.Procedure);

            if (error.Number == 0 && error.Class == 0 && error.State == 1 && !hasProcedure)
            {
                sb.AppendFormat("[Server: Line {0}] ", error.LineNumber);
            }
            else
            {
                sb.AppendFormat("[Server: Msg {0}, Level {1}, State {2}", error.Number, error.Class, error.State);

                if (hasProcedure)
                {
                    sb.AppendFormat(", Procedure: {0}", error.Procedure);
                }

                sb.AppendFormat(", Line {0}] ", error.LineNumber);
            }

            sb.Append(error.Message);
            return sb.ToString();
        }
    }
}