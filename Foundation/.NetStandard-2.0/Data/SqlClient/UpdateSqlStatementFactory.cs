using System.Collections.Generic;
using System.Linq;
using Foundation.Assertions;

namespace Foundation.Data.SqlClient
{
    public static class UpdateSqlStatementFactory
    {
        public static SqlStatement Row(string tableName, IReadOnlyCollection<string> columnNames, IReadOnlyCollection<string> row, string where)
        {
            Assert.IsNotNull(tableName);
            Assert.IsNotNull(columnNames);
            Assert.IsNotNull(row);
            Assert.IsTrue(columnNames.Count > 0);
            Assert.IsTrue(columnNames.Count == row.Count);

            var sqlStatementBuilder = new SqlStatementBuilder();
            sqlStatementBuilder.Add($"update {tableName}");
            sqlStatementBuilder.Add("set");

            var columns = columnNames.Zip(row, (columnName, value) => new
                {
                    ColumnName = columnName,
                    Value = value
                })
                .Select(i => $"    {i.ColumnName} = {i.Value}")
                .ToList();
            sqlStatementBuilder.AddRange(columns);
            sqlStatementBuilder.Add(where);

            return sqlStatementBuilder.ToSqlStatement();
        }
    }
}