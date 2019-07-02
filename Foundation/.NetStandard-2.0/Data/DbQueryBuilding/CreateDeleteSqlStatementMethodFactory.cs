using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Foundation.Collections;
using Foundation.Data.SqlClient;
using Foundation.Linq;
using Foundation.Text;

namespace Foundation.Data.DbQueryBuilding
{
    public static class CreateDeleteSqlStatementMethodFactory
    {
        public static ReadOnlyCollection<Line> Create(string schema, string table, IReadOnlyCollection<Column> whereColumns)
        {
            var arguments = whereColumns.Select(column =>
            {
                var csharpTypeName = GetCSharpTypeName(column);
                return $"{csharpTypeName} {column.ColumnName.ToCamelCase()}";
            }).Join(", ");

            var textBuilder = new TextBuilder();
            textBuilder.Add($"public static ReadOnlyCollection<Line> CreateDeleteSqlStatement({arguments})");
            using (textBuilder.AddCSharpBlock())
            {
                textBuilder.Add("var whereColumns = new[]");
                using (textBuilder.AddCSharpBlock())
                {
                    foreach (var item in whereColumns.SelectIndexed())
                    {
                        if (item.Index > 0)
                            textBuilder.AddToLastLine(",");
                        var column = item.Value;
                        var method = MethodName.GetToSqlConstantMethodName(column.SqlDataTypeName, column.IsNullable);
                        textBuilder.Add($"new ColumnNameValue(\"{column.ColumnName}\", {column.ColumnName.ToCamelCase()}.{method}())");
                    }
                }

                textBuilder.AddToLastLine(";");
                textBuilder.Add($"var deleteSqlStatement = DeleteSqlStatementFactory.Create(\"{schema}.{table}\", whereColumns);");
                textBuilder.Add("return deleteSqlStatement;");
            }

            return textBuilder.ToLines();
        }

        private static string GetCSharpTypeName(Column column)
        {
            var csharpTypeName = SqlDataTypeArray.SqlDataTypes.First(i => i.SqlDataTypeName == column.SqlDataTypeName).CSharpTypeName;
            var csharpType = CSharpTypeArray.CSharpTypes.First(i => i.Name == csharpTypeName);
            if (column.IsNullable && csharpType.Type.IsValueType)
                csharpTypeName += "?";
            return csharpTypeName;
        }
    }
}