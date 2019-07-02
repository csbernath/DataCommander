using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Foundation.Assertions;
using Foundation.Core;
using Foundation.Text;

namespace Foundation.Data.SqlClient.SqlStatementFactories
{
    public static class UpdateSqlStatementFactory
    {
        public static ReadOnlyCollection<Line> Create(string table, IReadOnlyCollection<ColumnNameValue> setColumns,
            IReadOnlyCollection<ColumnNameValue> whereColumns)
        {
            Assert.IsTrue(!table.IsNullOrEmpty());
            Assert.IsNotNull(setColumns);
            Assert.IsTrue(setColumns.Count > 0);
            Assert.IsNotNull(whereColumns);
            Assert.IsTrue(whereColumns.Count > 0);
            Assert.IsTrue(whereColumns.All(column => !column.Value.IsNullOrEmpty()));

            var textBuilder = new TextBuilder();
            textBuilder.Add($"update {table}");
            textBuilder.Add("set");
            using (textBuilder.Indent(1))
                textBuilder.Add(setColumns.Join(","));

            textBuilder.Add($"where");
            using (textBuilder.Indent(1))
                textBuilder.Add(whereColumns.Join("and"));

            return textBuilder.ToLines();
        }
    }
}