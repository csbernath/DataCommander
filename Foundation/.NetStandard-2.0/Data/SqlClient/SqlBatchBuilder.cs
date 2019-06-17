using System.Collections.Generic;
using System.Linq;
using Foundation.Text;

namespace Foundation.Data.SqlClient
{
    public sealed class SqlBatchBuilder
    {
        private readonly List<SqlStatement> _sqlStatements = new List<SqlStatement>();
        public void Add(SqlStatement sqlStatement) => _sqlStatements.Add(sqlStatement);
        public void AddRange(IEnumerable<SqlStatement> sqlStatements) => _sqlStatements.AddRange(sqlStatements);

        public string ToCommandText()
        {
            var commandText = _sqlStatements.Select(s => s.ToCommandText()).Join("\r\n");
            return commandText;
        }
    }
}