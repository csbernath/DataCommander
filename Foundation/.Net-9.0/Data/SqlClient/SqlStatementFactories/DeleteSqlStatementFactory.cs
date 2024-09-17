using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Foundation.Assertions;
using Foundation.Core;
using Foundation.Text;

namespace Foundation.Data.SqlClient.SqlStatementFactories;

public static class DeleteSqlStatementFactory
{
    public static ReadOnlyCollection<Line> Create(string table, IReadOnlyCollection<ColumnNameValue> whereColumns)
    {
        Assert.IsTrue(!table.IsNullOrEmpty());
        ArgumentNullException.ThrowIfNull(whereColumns);
        Assert.IsTrue(whereColumns.Count > 0);
        TextBuilder textBuilder = new TextBuilder();
        textBuilder.Add($"delete {table}");
        textBuilder.Add("where");
        using (textBuilder.Indent(1))
            textBuilder.Add(whereColumns.Join(" and"));
        return textBuilder.ToLines();
    }
}