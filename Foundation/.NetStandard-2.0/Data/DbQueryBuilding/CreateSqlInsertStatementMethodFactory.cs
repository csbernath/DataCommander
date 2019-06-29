using System.Collections.ObjectModel;
using Foundation.Linq;
using Foundation.Text;

namespace Foundation.Data.DbQueryBuilding
{
    public static class CreateSqlInsertStatementMethodFactory
    {
        public static ReadOnlyCollection<Line> Create(string schema, string table, ReadOnlyCollection<Column> columns)
        {
            var textBuilder = new TextBuilder();
            textBuilder.Add($"public static ReadOnlyCollection<Line> CreateInsertSqlStatement(IEnumerable<{table}> records)");
            using (textBuilder.AddCSharpBlock())
            {
                //textBuilder.Add($"var sqlTable = new SqlTable(\"{_owner}\",\"{_name}\", new[]");
                //using (textBuilder.AddCSharpBlock())
                //{
                //    sequence = new Sequence();
                //    foreach (var column in columns)
                //    {
                //        var last = sequence.Next() == columns.Count - 1;
                //        var name = column.ColumnName;
                //        var separator = !last ? "," : null;
                //        textBuilder.Add($"\"{name}\"{separator}");
                //    }
                //}

                //textBuilder.AddToLastLine(".ToReadOnlyCollection());");
                //textBuilder.Add(Line.Empty);

                textBuilder.Add("var columns = new[]");
                using (textBuilder.AddCSharpBlock())
                {
                    foreach (var indexedColumn in columns.SelectIndexed())
                    {
                        if (indexedColumn.Index > 0)
                            textBuilder.AddToLastLine(",");
                        var column = indexedColumn.Value;
                        textBuilder.Add($"\"{column.ColumnName}\"");
                    }
                }

                textBuilder.AddToLastLine(";");
                textBuilder.Add($"var rows = records.Select(record => new[]");
                using (textBuilder.AddCSharpBlock())
                {
                    foreach (var indexedColumn in columns.SelectIndexed())
                    {
                        if (indexedColumn.Index > 0)
                            textBuilder.AddToLastLine(",");
                        var column = indexedColumn.Value;
                        var methodName = MethodName.GetToSqlConstantMethodName(column.SqlDataTypeName, column.IsNullable);
                        textBuilder.Add($"record.{column.ColumnName}.{methodName}()");
                    }
                }

                textBuilder.AddToLastLine(").ToReadOnlyCollection();");
                textBuilder.Add($"var insertSqlStatement = InsertSqlStatementFactory.Create(\"{schema}.{table}\", columns, rows);");
                textBuilder.Add("return insertSqlStatement;");
            }

            return textBuilder.ToLines();
        }
    }
}