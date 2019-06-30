using System.Collections.Generic;
using System.Collections.ObjectModel;
using Foundation.Assertions;
using Foundation.Core;
using Foundation.Linq;
using Foundation.Text;

namespace Foundation.Data.SqlClient.SqlStatementFactories
{
    public static class UpdateSqlStatementFactory
    {
        public sealed class Column
        {
            public readonly string Name;
            public readonly string Value;

            public Column(string name, string value)
            {
                Assert.IsTrue(!name.IsNullOrEmpty());

                Name = name;
                Value = value;
            }
        }

        public static ReadOnlyCollection<Line> Create(string table, Column identifier, IReadOnlyCollection<Column> columns)
        {
            Assert.IsTrue(!table.IsNullOrEmpty());
            Assert.IsNotNull(identifier);
            Assert.IsTrue(!identifier.Value.IsNullOrEmpty());
            Assert.IsNotNull(columns);
            Assert.IsTrue(columns.Count > 0);

            var textBuilder = new TextBuilder();
            textBuilder.Add($"update {table}");
            textBuilder.Add("set");
            using (textBuilder.Indent(1))
                foreach (var item in columns.SelectIndexed())
                {
                    if (item.Index > 0)
                        textBuilder.AddToLastLine(",");
                    var column = item.Value;
                    textBuilder.Add($"{column.Name} = {column.Value}");
                }

            textBuilder.Add($"where");
            using (textBuilder.Indent(1))
                textBuilder.Add($"{identifier.Name} = {identifier.Value}");

            return textBuilder.ToLines();
        }
    }
}