using System.Collections.Generic;
using Foundation.Linq;

namespace Foundation.Data.SqlClient
{
    public sealed class SqlBatchBuilder
    {
        private readonly List<SqlStatement> _sqlStatements = new List<SqlStatement>();
        public void Add(SqlStatement sqlStatement) => _sqlStatements.Add(sqlStatement);
        public void AddRange(IEnumerable<SqlStatement> sqlStatements) => _sqlStatements.AddRange(sqlStatements);
        public SqlBatch ToSqlBatch() => new SqlBatch(_sqlStatements.ToReadOnlyCollection());
    }
}