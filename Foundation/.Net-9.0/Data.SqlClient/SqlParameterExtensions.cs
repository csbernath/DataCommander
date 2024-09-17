using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;

namespace Foundation.Data.SqlClient;

public static class SqlParameterExtensions
{
    public static string GetDataTypeName(this SqlParameter parameter)
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(parameter.SqlDbType.ToString().ToLowerInvariant());

        switch (parameter.SqlDbType)
        {
            case SqlDbType.Decimal:
                if (parameter.Scale == 0)
                    stringBuilder.AppendFormat("({0})", parameter.Precision);
                else
                    stringBuilder.AppendFormat("({0},{1})", parameter.Precision, parameter.Scale);
                break;

            case SqlDbType.Char:
            case SqlDbType.NChar:
            case SqlDbType.VarChar:
            case SqlDbType.NVarChar:
                int size = parameter.Size;
                string sizeString;

                if (size == -1 || size == int.MaxValue)
                    sizeString = "max";
                else
                    sizeString = size.ToString();

                stringBuilder.AppendFormat("({0})", sizeString);
                break;
        }

        return stringBuilder.ToString();
    }
}