using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Foundation.Assertions;
using Foundation.Text;

namespace Foundation.Data.SqlClient
{
    public static class InsertSqlStatementFactory
    {
        public static ReadOnlyCollection<Line> Row(string schemaName, string tableName, IReadOnlyCollection<string> columnNames,
            IReadOnlyCollection<string> row) => Rows(schemaName, tableName, columnNames, new[] {row});

        public static ReadOnlyCollection<Line> Rows(string schemaName, string tableName, IReadOnlyCollection<string> columnNames,
            IReadOnlyCollection<IReadOnlyCollection<string>> rows)
        {
            Assert.IsNotNull(tableName);
            Assert.IsNotNull(columnNames);
            Assert.IsNotNull(rows);
            Assert.IsTrue(columnNames.Count > 0);
            Assert.IsTrue(rows.Count > 0);
            Assert.IsTrue(rows.All(row => row.Count == columnNames.Count));

            var indentedTextBuilder = new TextBuilder();
            indentedTextBuilder.Add($"insert into {schemaName}.{tableName}({columnNames.Join(",")})");
            indentedTextBuilder.Add("values");

            using (indentedTextBuilder.Indent(1))
            {
                foreach (var row in rows)
                {
                    var values = row.Join(",");
                    indentedTextBuilder.Add($"({values})");
                }
            }

            return indentedTextBuilder.ToReadOnlyCollection();
        }
    }
}