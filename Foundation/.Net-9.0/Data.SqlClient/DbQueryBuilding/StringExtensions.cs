using Foundation.Core;

namespace Foundation.Data.SqlClient.DbQueryBuilding;

public static class StringExtensions
{
    public static string ToCamelCase(this string pascalCase) => !pascalCase.IsNullOrEmpty()
            ? char.ToLower(pascalCase[0]) + pascalCase[1..]
            : pascalCase;

    public static string ToPascalCase(this string camelCase) => !camelCase.IsNullOrEmpty()
            ? char.ToUpper(camelCase[0]) + camelCase[1..]
            : camelCase;
}