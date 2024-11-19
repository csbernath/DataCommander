using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Foundation.Linq;
using Foundation.Text;

namespace Foundation.Data.SqlClient.DbQueryBuilding;

public static class CreateUpdateSqlStatementMethodFactory
{
    public static ReadOnlyCollection<Line> Create(string schema, string table, Column identifierColumn, Column? versionColumn,
        IReadOnlyCollection<Column> columns)
    {
        List<string> arguments =
        [
            $"{table} record"
        ];
        if (versionColumn != null)
        {
            var csharpTypeName = SqlDataTypeRepository.SqlDataTypes.First(i => i.SqlDataTypeName == versionColumn.SqlDataTypeName).CSharpTypeName;
            arguments.Add($"{csharpTypeName} expected{versionColumn.ColumnName}");
        }

        var textBuilder = new TextBuilder();
        textBuilder.Add($"public static ReadOnlyCollection<Line> CreateUpdateSqlStatement({arguments.Join(", ")})");
        using (textBuilder.AddCSharpBlock())
        {
            textBuilder.Add("var setColumns = new []");
            using (textBuilder.AddCSharpBlock())
                foreach (var item in columns.SelectIndexed())
                {
                    if (item.Index > 0)
                        textBuilder.AddToLastLine(",");
                    var column = item.Value;
                    var method = MethodName.GetToSqlConstantMethodName(column!.SqlDataTypeName, column.IsNullable);
                    textBuilder.Add($"new ColumnNameValue(\"{column.ColumnName}\", record.{column.ColumnName}.{method}())");
                }

            textBuilder.AddToLastLine(";");

            textBuilder.Add("var whereColumns = new[]");
            using (textBuilder.AddCSharpBlock())
            {
                var method = MethodName.GetToSqlConstantMethodName(identifierColumn.SqlDataTypeName, identifierColumn.IsNullable);
                textBuilder.Add($"new ColumnNameValue(\"{identifierColumn.ColumnName}\", record.{identifierColumn.ColumnName}.{method}())");
                if (versionColumn != null)
                {
                    textBuilder.AddToLastLine(",");
                    method = MethodName.GetToSqlConstantMethodName(versionColumn.SqlDataTypeName, versionColumn.IsNullable);
                    textBuilder.Add($"new ColumnNameValue(\"{versionColumn.ColumnName}\", expected{versionColumn.ColumnName}.{method}())");
                }
            }

            textBuilder.AddToLastLine(";");
            textBuilder.Add($"var updateSqlStatement = UpdateSqlStatementFactory.Create(\"{schema}.{table}\", setColumns, whereColumns);");
            textBuilder.Add($"var validation = ValidationFactory.Create(\"update {schema}.{table} failed\");");
            textBuilder.Add("var textBuilder = new TextBuilder();");
            textBuilder.Add("textBuilder.Add(updateSqlStatement);");
            textBuilder.Add("textBuilder.Add(validation);");
            textBuilder.Add("return textBuilder.ToLines();");
        }

        return textBuilder.ToLines();
    }
}