using System.Collections.ObjectModel;
using Foundation.Assertions;
using Foundation.Core;
using Foundation.Text;

namespace Foundation.Data.SqlClient
{
    public static class DeleteSqlStatementFactory
    {
        public static ReadOnlyCollection<Line> Create(string table, UpdateSqlStatementFactory.Column identifier)
        {
            Assert.IsTrue(!table.IsNullOrEmpty());
            Assert.IsNotNull(identifier);
            Assert.IsTrue(!identifier.Value.IsNullOrEmpty());
            var textBuilder = new TextBuilder();
            textBuilder.Add($"delete {table}");
            textBuilder.Add("where");

            using (textBuilder.Indent(1))
                textBuilder.Add($"{identifier.Name} = {identifier.Value}");

            return textBuilder.ToLines();
        }
    }
}