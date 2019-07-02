using System.Collections.Generic;
using System.Collections.ObjectModel;
using Foundation.Linq;
using Foundation.Text;

namespace Foundation.Data.DbQueryBuilding
{
    public static class CreateUpdateSqlStatementMethodFactory
    {
        public static ReadOnlyCollection<Line> Create(string schema, string table, IReadOnlyCollection<Column> setColumns,
            IReadOnlyCollection<Column> whereColumns)
        {
            var textBuilder = new TextBuilder();

            textBuilder.Add($"public static ReadOnlyCollection<Line> CreateUpdateSqlStatement({table} record, long expectedVersion)");
            using (textBuilder.AddCSharpBlock())
            {
                textBuilder.Add("var setColumns = new []");
                using (textBuilder.AddCSharpBlock())
                    foreach (var item in setColumns.SelectIndexed())
                    {
                        if (item.Index > 0)
                            textBuilder.AddToLastLine(",");
                        var column = item.Value;
                        var method = MethodName.GetToSqlConstantMethodName(column.SqlDataTypeName, column.IsNullable);
                        textBuilder.Add($"new ColumnNameValue(\"{column.ColumnName}\", record.{column.ColumnName}.{method}())");
                    }

                textBuilder.AddToLastLine(";");

                textBuilder.Add("var whereColumns = new[]");
                using (textBuilder.AddCSharpBlock())
                    foreach (var item in whereColumns.SelectIndexed())
                    {
                        if (item.Index > 0)
                            textBuilder.AddToLastLine(",");
                        var column = item.Value;
                        var method = MethodName.GetToSqlConstantMethodName(column.SqlDataTypeName, column.IsNullable);
                        textBuilder.Add(column.ColumnName == "Version"
                            ? $"new ColumnNameValue(\"{column.ColumnName}\", expected{column.ColumnName}.{method}())"
                            : $"new ColumnNameValue(\"{column.ColumnName}\", record.{column.ColumnName}.{method}())");
                    }

                textBuilder.AddToLastLine(";");

                //var variable = VariableFactory.Create("setColumns", setColumns);
                //textBuilder.Add(variable);
                //variable = VariableFactory.Create("whereColumns", whereColumns);
                //textBuilder.Add(variable);
                textBuilder.Add($"var updateSqlStatement = UpdateSqlStatementFactory.Create(\"{schema}.{table}\", setColumns, whereColumns);");
                textBuilder.Add("return updateSqlStatement;");
            }

            return textBuilder.ToLines();
        }
    }
}