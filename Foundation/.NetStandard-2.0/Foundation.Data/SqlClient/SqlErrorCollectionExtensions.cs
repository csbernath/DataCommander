using System.Data.SqlClient;
using System.Text;

namespace Foundation.Data.SqlClient
{
    public static class SqlErrorCollectionExtensions
    {
        public static string ToLogString(this SqlErrorCollection errors)
        {
            string message = null;

            if (errors != null)
            {
                var sb = new StringBuilder();

                foreach (SqlError error in errors)
                {
                    var s = error.ToLogString();
                    sb.AppendLine(s);
                }

                message = sb.ToString();
            }

            return message;
        }
    }
}