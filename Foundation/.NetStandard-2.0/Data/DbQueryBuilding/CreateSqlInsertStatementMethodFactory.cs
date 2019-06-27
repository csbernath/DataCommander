using System.Collections.ObjectModel;
using Foundation.Data.SqlClient;
using Foundation.Linq;
using Foundation.Text;

namespace Foundation.Data.DbQueryBuilding
{
    public static class CreateSqlInsertStatementMethodFactory
    {
        public sealed class Column
        {
            public readonly string ColumnName;
            public readonly string SqlDataTypeName;
            public readonly bool IsNullable;

            public Column(string columnName, string sqlDataTypeName, bool isNullable)
            {
                ColumnName = columnName;
                SqlDataTypeName = sqlDataTypeName;
                IsNullable = isNullable;
            }
        }

        public static ReadOnlyCollection<Line> Create(string schema, string table, ReadOnlyCollection<Column> columns)
        {
            var textBuilder = new TextBuilder();
            textBuilder.Add($"public static ReadOnlyCollection<Line> CreateInsertSqlStatement(IEnumerable<{table}> source)");
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
                textBuilder.Add($"var rows = source.Select(item => new[]");
                using (textBuilder.AddCSharpBlock())
                {
                    foreach (var indexedColumn in columns.SelectIndexed())
                    {
                        if (indexedColumn.Index > 0)
                            textBuilder.AddToLastLine(",");
                        var column = indexedColumn.Value;
                        string methodName;
                        switch (column.SqlDataTypeName)
                        {
                            case SqlDataTypeName.NVarChar:
                                methodName = column.IsNullable ? "ToNullableNVarChar" : "ToNVarChar";
                                break;
                            case SqlDataTypeName.VarChar:
                                methodName = column.IsNullable ? "ToNullableVarChar" : "ToVarChar";
                                break;
                            default:
                                methodName = "ToSqlConstant";
                                break;
                        }

                        textBuilder.Add($"item.{column.ColumnName}.{methodName}()");
                    }
                }

                textBuilder.AddToLastLine(").ToReadOnlyCollection();");
                textBuilder.Add(
                    $"var insertSqlStatement = InsertSqlStatementFactory.CreateInsertSqlStatement(\"{schema}\", \"{table}\", columns, rows);");
                textBuilder.Add("return insertSqlStatement;");
            }

            return textBuilder.ToLines();
        }
    }
}