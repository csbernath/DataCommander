using System.Collections.ObjectModel;
using System.Linq;
using Foundation.Text;

namespace Foundation.Data.SqlClient
{
    public sealed class SqlBatch
    {
        private readonly ReadOnlyCollection<SqlStatement> _sqlStatements;
        internal SqlBatch(ReadOnlyCollection<SqlStatement> sqlStatements) => _sqlStatements = sqlStatements;

        public string ToCommandText()
        {
            var commandText = _sqlStatements.Select(s => s.ToCommandText()).Join("\r\n");
            return commandText;
        }
    }
}