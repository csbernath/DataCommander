using System.Collections.Generic;
using System.Linq;
using System.Text;
using Foundation.Assertions;
using Foundation.Core;
using Foundation.Text;

namespace Foundation.Data.SqlClient.SqlStatementFactories;

public static class ColumnNameValueExtensions
{
    extension(IReadOnlyCollection<ColumnNameValue> columns)
    {
        public IReadOnlyCollection<Line> Join(string separator)
        {
            Assert.IsTrue(columns.All(column => !column.Value.IsNullOrEmpty()));

            var last = columns.Count - 1;
            var items = columns.Select((column, index) =>
                {
                    var stringBuilder = new StringBuilder();
                    stringBuilder.Append($"{column.Name} = {column.Value}");

                    if (index < last)
                        stringBuilder.Append(separator);
                    return new Line(0, stringBuilder.ToString());
                })
                .ToArray();
            return items;
        }
    }
}