using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Foundation.Assertions;
using Foundation.Core;
using Foundation.Text;

namespace Foundation.Data.SqlClient
{
    public static class InsertSqlStatementFactory
    {
        public static ReadOnlyCollection<Line> CreateInsertSqlStatement(string table, IReadOnlyCollection<string> columns,
            IReadOnlyCollection<IReadOnlyCollection<string>> rows)
        {
            Assert.IsTrue(!table.IsNullOrEmpty());
            Assert.IsTrue(!table.IsNullOrEmpty());
            Assert.IsNotNull(columns);
            Assert.IsNotNull(rows);
            Assert.IsTrue(columns.Count > 0);
            Assert.IsTrue(rows.All(row => row.Count == columns.Count));

            var textBuilder = new TextBuilder();
            textBuilder.Add($"insert into {table}({columns.Join(",")})");
            textBuilder.Add("values");

            using (textBuilder.Indent(1))
            {
                var first = true;
                foreach (var row in rows)
                {
                    if (first)
                        first = false;
                    else
                        textBuilder.AddToLastLine(",");

                    var values = row.Join(",");
                    textBuilder.Add($"({values})");
                }
            }

            return textBuilder.ToLines();
        }
    }
}