using System.Collections.Generic;
using System.Linq;
using Foundation.Assertions;
using Foundation.Text;

namespace Foundation.Data.SqlClient
{
    public static class InsertSqlStatementFactory
    {
        public static SqlStatement Row(string tableName, IReadOnlyCollection<string> columnNames, IReadOnlyCollection<string> row) =>
            Rows(tableName, columnNames, new[] {row});

        public static SqlStatement Rows(string tableName, IReadOnlyCollection<string> columnNames,
            IReadOnlyCollection<IReadOnlyCollection<string>> rows)
        {
            Assert.IsNotNull(tableName);
            Assert.IsNotNull(columnNames);
            Assert.IsNotNull(rows);
            Assert.IsTrue(columnNames.Count > 0);
            Assert.IsTrue(rows.Count > 0);
            Assert.IsTrue(rows.All(row => row.Count == columnNames.Count));

            var sqlStatementBuilder = new SqlStatementBuilder();
            sqlStatementBuilder.Add($"insert into {tableName}({columnNames.Join(",")})");
            sqlStatementBuilder.Add("values");
            var rowLines = rows.Select(row => $"    ({row.Join(",")})").ToList();
            sqlStatementBuilder.AddRange(rowLines);
            return sqlStatementBuilder.ToSqlStatement();
        }
    }
}