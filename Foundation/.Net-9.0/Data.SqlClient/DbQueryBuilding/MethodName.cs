namespace Foundation.Data.SqlClient.DbQueryBuilding;

internal static class MethodName
{
    public static string GetToSqlConstantMethodName(string sqlDataTypeName, bool isNullable)
    {
        string methodName = sqlDataTypeName switch
        {
            SqlDataTypeName.NVarChar => isNullable ? "ToNullableNVarChar" : "ToNVarChar",
            SqlDataTypeName.VarChar => isNullable ? "ToNullableVarChar" : "ToVarChar",
            _ => "ToSqlConstant",
        };
        return methodName;
    }
}