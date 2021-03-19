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
                var stringBuilder = new StringBuilder();
                foreach (SqlError error in errors)
                {
                    var s = error.ToLogString();
                    stringBuilder.AppendLine(s);
                }

                message = stringBuilder.ToString();
            }

            return message;
        }
    }
}