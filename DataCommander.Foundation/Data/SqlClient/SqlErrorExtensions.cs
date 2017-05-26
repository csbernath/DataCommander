using System.Data.SqlClient;
using System.Text;

namespace Foundation.Data.SqlClient
{
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
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(error != null);
#endif

            var sb = new StringBuilder();

            var hasProcedure = !string.IsNullOrEmpty(error.Procedure);

            if (error.Number == 0 && error.Class == 0 && error.State == 1)
            {
                if (hasProcedure)
                {
                    sb.AppendFormat("[Server: Procedure {0}, Line {1}] ", error.Procedure, error.LineNumber);
                }
                else
                {
                    sb.AppendFormat("[Server: Line {0}] ", error.LineNumber);
                }
            }
            else if (error.Class == 0 && error.State == 1 && error.LineNumber == 1)
            {
                sb.AppendFormat("[Server: Msg {0}] ", error.Number);
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