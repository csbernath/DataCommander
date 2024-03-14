using System;
using System.Collections.Generic;
using System.Linq;

namespace Foundation.Data.SqlEngine;

public static class TableExtensions
{
    public static Table GroupJoin(
        this Table outerTable,
        Table innerTable,
        Func<object[], Key> outerKeySelector,
        Func<object[], Key> innerKeySelector,
        IGroupJoinResultSelector groupJoinResultSelector,
        IEqualityComparer<Key> comparer)
    {
        var groupJoinResult = outerTable.Rows
            .GroupJoin(innerTable.Rows, outerKeySelector, innerKeySelector, groupJoinResultSelector.Select, comparer);
        var resultRows = groupJoinResult.SelectMany(r => r.ToArray());
        var resultTable = new Table(groupJoinResultSelector.ResultTableName, groupJoinResultSelector.ResultColumns,
            resultRows);
        return resultTable;
    }

    public static Table OrderBy(
        this Table table,
        Func<object[], Key> keySelector,
        IComparer<Key> comparer)
    {
        var rows = table.Rows
            .OrderBy(keySelector, comparer);
        return new Table(null, table.Columns, rows);
    }

    public static Table Where(
        this Table table,
        string resultTableName,
        Func<object[], bool> predicate)
    {
        var rows = table.Rows
            .Where(predicate);
        return new Table(resultTableName, table.Columns, rows);
    }
}