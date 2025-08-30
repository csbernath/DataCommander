using System;
using System.Text;
using Microsoft.Data.SqlClient;

namespace Foundation.Data.SqlClient;

public static class SqlErrorExtensions
{
    public static string GetHeader(this SqlError error)
    {
        ArgumentNullException.ThrowIfNull(error);
        var stringBuilder = new StringBuilder();
        var hasProcedure = !string.IsNullOrEmpty(error.Procedure);

        if (error.Number == 0 && error.Class == 0 && error.State == 1)
        {
            if (hasProcedure)
                stringBuilder.Append($"Server: Procedure {error.Procedure}, Line {error.LineNumber}");
            else
                stringBuilder.Append($"Server: Line {error.LineNumber}");
        }
        else if (error.Class == 0 && error.State == 1 && error.LineNumber == 1)
            stringBuilder.Append($"Server: Msg {error.Number}");
        else
        {
            stringBuilder.Append($"Server: Msg {error.Number}, Level {error.Class}, State {error.State}");

            if (hasProcedure)
                stringBuilder.Append($", Procedure: {error.Procedure}");

            stringBuilder.Append($", Line {error.LineNumber}");
        }

        return stringBuilder.ToString();
    }

    public static string ToLogString(this SqlError error)
    {
        ArgumentNullException.ThrowIfNull(error);
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(error.GetHeader());
        stringBuilder.Append(error.Message);
        return stringBuilder.ToString();
    }
}