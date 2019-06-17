using System.Collections.Generic;
using Foundation.Linq;

namespace Foundation.Data.SqlClient
{
    public sealed class SqlStatementBuilder
    {
        private readonly List<string> _items = new List<string>();
        public void Add(string item) => _items.Add(item);
        public void AddRange(IEnumerable<string> items) => _items.AddRange(items);
        public SqlStatement ToSqlStatement() => new SqlStatement(_items.ToReadOnlyCollection());
    }
}