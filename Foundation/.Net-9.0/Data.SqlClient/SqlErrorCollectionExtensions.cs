using System.Text;
using Microsoft.Data.SqlClient;

namespace Foundation.Data.SqlClient;

public static class SqlErrorCollectionExtensions
{
    public static string ToLogString(this SqlErrorCollection errors)
    {
        string message = null;

        if (errors != null)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (SqlError error in errors)
            {
                string s = error.ToLogString();
                stringBuilder.AppendLine(s);
            }

            message = stringBuilder.ToString();
        }

        return message;
    }
}