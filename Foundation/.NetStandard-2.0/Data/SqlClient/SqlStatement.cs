using System.Collections.Generic;
using Foundation.Text;

namespace Foundation.Data.SqlClient
{
    public sealed class SqlStatement
    {
        private readonly List<string> _items;
        internal SqlStatement(List<string> items) => _items = items;
        public string ToCommandText() => _items.Join("\r\n");
    }
}
