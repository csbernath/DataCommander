using System.Collections.Generic;
using System.Linq;
using Foundation.Assertions;
using Foundation.Text;

namespace Foundation.Data.SqlClient
{
    public static class UpdateSqlStatementFactory
    {
        public static IReadOnlyCollection<Line> Row(string tableName, IReadOnlyCollection<string> columnNames, IReadOnlyCollection<string> row,
            IReadOnlyCollection<Line> where)
        {
            Assert.IsNotNull(tableName);
            Assert.IsNotNull(columnNames);
            Assert.IsNotNull(row);
            Assert.IsTrue(columnNames.Count > 0);
            Assert.IsTrue(columnNames.Count == row.Count);

            var indentedTextBuilder = new TextBuilder();
            indentedTextBuilder.Add($"update {tableName}");
            indentedTextBuilder.Add("set");
            using (indentedTextBuilder.Indent(1))
            {
                var items = columnNames.Zip(row, (columnName, value) => new
                {
                    ColumnName = columnName,
                    Value = value
                }).ToList();

                foreach (var item in items)
                    indentedTextBuilder.Add($"{item.ColumnName} = {item.Value}");
            }

            indentedTextBuilder.Add(where);

            return indentedTextBuilder.ToReadOnlyCollection();
        }
    }
}