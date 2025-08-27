using System.Text;
using Microsoft.Data.SqlClient;

namespace Foundation.Data.SqlClient;

public static class SqlErrorCollectionExtensions
{
    extension(SqlErrorCollection errors)
    {
        public string? ToLogString()
        {
            string? message = null;

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