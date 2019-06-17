using System.Collections.ObjectModel;
using Foundation.Text;

namespace Foundation.Data.SqlClient
{
    public sealed class SqlStatement
    {
        private readonly ReadOnlyCollection<string> _items;
        internal SqlStatement(ReadOnlyCollection<string> items) => _items = items;
        public string ToCommandText() => _items.Join("\r\n");
    }
}