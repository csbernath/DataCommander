using Foundation.Data.SqlClient;

namespace Foundation.Data.DbQueryBuilding;

internal static class MethodName
{
    public static string GetToSqlConstantMethodName(string sqlDataTypeName, bool isNullable)
    {
        string methodName;
        switch (sqlDataTypeName)
        {
            case SqlDataTypeName.NVarChar:
                methodName = isNullable ? "ToNullableNVarChar" : "ToNVarChar";
                break;
            case SqlDataTypeName.VarChar:
                methodName = isNullable ? "ToNullableVarChar" : "ToVarChar";
                break;
            default:
                methodName = "ToSqlConstant";
                break;
        }

        return methodName;
    }
}