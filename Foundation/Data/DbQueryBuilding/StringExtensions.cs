using Foundation.Core;

namespace Foundation.Data.DbQueryBuilding;

public static class StringExtensions
{
    public static string ToCamelCase(this string pascalCase)
    {
        return !pascalCase.IsNullOrEmpty()
            ? char.ToLower(pascalCase[0]) + pascalCase.Substring(1)
            : pascalCase;
    }

    public static string ToPascalCase(this string camelCase)
    {
        return !camelCase.IsNullOrEmpty()
            ? char.ToUpper(camelCase[0]) + camelCase.Substring(1)
            : camelCase;
    }
}