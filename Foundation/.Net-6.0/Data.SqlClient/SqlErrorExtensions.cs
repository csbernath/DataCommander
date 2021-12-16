using System.Text;
using Foundation.Assertions;
using Microsoft.Data.SqlClient;

namespace Foundation.Data.SqlClient;

public static class SqlErrorExtensions
{
    public static string GetHeader(this SqlError error)
    {
        Assert.IsNotNull(error);
        var stringBuilder = new StringBuilder();
        var hasProcedure = !string.IsNullOrEmpty(error.Procedure);

        if (error.Number == 0 && error.Class == 0 && error.State == 1)
        {
            if (hasProcedure)
                stringBuilder.AppendFormat("Server: Procedure {0}, Line {1}", error.Procedure, error.LineNumber);
            else
                stringBuilder.AppendFormat("Server: Line {0}", error.LineNumber);
        }
        else if (error.Class == 0 && error.State == 1 && error.LineNumber == 1)
            stringBuilder.AppendFormat("Server: Msg {0}", error.Number);
        else
        {
            stringBuilder.AppendFormat("Server: Msg {0}, Level {1}, State {2}", error.Number, error.Class, error.State);

            if (hasProcedure)
                stringBuilder.AppendFormat(", Procedure: {0}", error.Procedure);

            stringBuilder.AppendFormat(", Line {0}", error.LineNumber);
        }

        return stringBuilder.ToString();
    }

    public static string ToLogString(this SqlError error)
    {
        Assert.IsNotNull(error);
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(error.GetHeader());
        stringBuilder.Append(error.Message);
        return stringBuilder.ToString();
    }
}