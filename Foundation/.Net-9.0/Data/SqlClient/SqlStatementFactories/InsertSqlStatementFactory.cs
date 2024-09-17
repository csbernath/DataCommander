using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Foundation.Assertions;
using Foundation.Core;
using Foundation.Linq;
using Foundation.Text;

namespace Foundation.Data.SqlClient.SqlStatementFactories;

public static class InsertSqlStatementFactory
{
    public static ReadOnlyCollection<Line> Create(string table, IReadOnlyCollection<string> columns, IReadOnlyCollection<IReadOnlyCollection<string>> rows)
    {
        Assert.IsTrue(!table.IsNullOrEmpty());
        Assert.IsTrue(!table.IsNullOrEmpty());
        ArgumentNullException.ThrowIfNull(columns);
        ArgumentNullException.ThrowIfNull(rows);
        Assert.IsTrue(columns.Count > 0);
        Assert.IsTrue(rows.All(row => row.Count == columns.Count));

        TextBuilder textBuilder = new TextBuilder();
        textBuilder.Add($"insert into {table}({columns.Join(",")})");
        textBuilder.Add("values");

        using (textBuilder.Indent(1))
        {
            foreach (IndexedItem<IReadOnlyCollection<string>> indexedRow in rows.SelectIndexed())
            {
                if (indexedRow.Index > 0)
                    textBuilder.AddToLastLine(",");

                string values = indexedRow.Value.Join(",");
                textBuilder.Add($"({values})");
            }
        }

        return textBuilder.ToLines();
    }
}