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
            sb.AppendFormat("Server: Msg {0}, Level {1}, State {2}", error.Number, error.Class, error.State);

            if (!string.IsNullOrEmpty(error.Procedure))
            {
                sb.AppendFormat(", Procedure: {0}", error.Procedure);
            }

            sb.AppendFormat(", Line {0}\r\n", error.LineNumber);
            sb.Append(error.Message);
            return sb.ToString();
        }
    }
}