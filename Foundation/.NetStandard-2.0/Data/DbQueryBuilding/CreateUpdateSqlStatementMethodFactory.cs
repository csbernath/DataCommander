using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Foundation.Linq;
using Foundation.Text;

namespace Foundation.Data.DbQueryBuilding
{
    public static class CreateUpdateSqlStatementMethodFactory
    {
        public static ReadOnlyCollection<Line> Create(string schema, string table, Column identifier, IReadOnlyCollection<Column> columns)
        {
            var textBuilder = new TextBuilder();

            textBuilder.Add($"public static ReadOnlyCollection<Line> CreateUpdateSqlStatement({table} record)");
            using (textBuilder.AddCSharpBlock())
            {
                textBuilder.Add(
                    $"var identifier = new ColumnNameValue(\"{identifier.ColumnName}\", record.{identifier.ColumnName}.{MethodName.GetToSqlConstantMethodName(identifier.SqlDataTypeName, identifier.IsNullable)}());");
                textBuilder.Add("var columns = new[]");
                using (textBuilder.AddCSharpBlock())
                {
                    var indexedColumns = columns
                        .Where(i => i.ColumnName != identifier.ColumnName)
                        .SelectIndexed();
                    foreach (var indexedColumn in indexedColumns)
                    {
                        if (indexedColumn.Index > 0)
                            textBuilder.AddToLastLine(",");
                        var column = indexedColumn.Value;
                        textBuilder.Add(
                            $"new ColumnNameValue(\"{column.ColumnName}\", record.{column.ColumnName}.{MethodName.GetToSqlConstantMethodName(column.SqlDataTypeName, column.IsNullable)}())");
                    }
                }

                textBuilder.AddToLastLine(";");
                textBuilder.Add($"var updateSqlStatement = UpdateSqlStatementFactory.Create(\"{schema}.{table}\", identifier, columns);");
                textBuilder.Add("return updateSqlStatement;");
            }

            return textBuilder.ToLines();
        }
    }
}