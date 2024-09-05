using System.Collections.Generic;

namespace Foundation.Data.SqlEngine;

public interface IRowSelector
{
    ColumnCollection ResultColumns { get; }
    IEnumerable<object> Select(object[] row);
}