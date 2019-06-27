using System.Collections.ObjectModel;
using Foundation.Text;

namespace Foundation.Data.DbQueryBuilding
{
    public static class CreateSqlUpdateStatementMethodFactory
    {
        public static ReadOnlyCollection<Line> Create(string schema, string table)
        {
            var textBuilder = new TextBuilder();

            textBuilder.Add("public static ReadOnlyCollection<Line> CreateSqlUpdateStatement(IEnumerable<{table>) source)");
            using (textBuilder.AddCSharpBlock())
            {
                textBuilder.Add($"var updateSqlStatement = UpdateSqlStatementFactory.Create(\"{schema}.{table}\", identifier, columns);");
                textBuilder.Add("return updateSqlStatement;");
            }

            return textBuilder.ToLines();
        }
    }
}