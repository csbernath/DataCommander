using System.Collections.Generic;

namespace Foundation.Data.SqlEngine;

public interface IGroupJoinResultSelector
{
    string ResultTableName { get; }
    ColumnCollection ResultColumns { get; }
    IEnumerable<object[]> Select(object[] innerRow, IEnumerable<object[]> outerRows);
}