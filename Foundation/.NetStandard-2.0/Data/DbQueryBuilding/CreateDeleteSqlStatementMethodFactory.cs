using System.Collections.ObjectModel;
using System.Linq;
using Foundation.Collections;
using Foundation.Data.SqlClient;
using Foundation.Text;

namespace Foundation.Data.DbQueryBuilding
{
    public static class CreateDeleteSqlStatementMethodFactory
    {
        public static ReadOnlyCollection<Line> Create(string schema, string table, Column identifier)
        {
            var csharpTypeName = SqlDataTypeArray.SqlDataTypes.First(i => i.SqlDataTypeName == identifier.SqlDataTypeName).CSharpTypeName;
            var csharpType = CSharpTypeArray.CSharpTypes.First(i => i.Name == csharpTypeName);
            if (identifier.IsNullable && csharpType.Type.IsValueType)
                csharpTypeName += "?";

            var textBuilder = new TextBuilder();
            textBuilder.Add(
                $"public static ReadOnlyCollection<Line> CreateDeleteSqlStatement({csharpTypeName} identifierValue)");
            using (textBuilder.AddCSharpBlock())
            {
                textBuilder.Add(
                    $"var identifier = new ColumnNameValue(\"{identifier.ColumnName}\", identifierValue.{MethodName.GetToSqlConstantMethodName(identifier.SqlDataTypeName, identifier.IsNullable)}());");
                textBuilder.Add($"var deleteSqlStatement = DeleteSqlStatementFactory.Create(\"{schema}.{table}\", identifier);");
                textBuilder.Add("return deleteSqlStatement;");
            }

            return textBuilder.ToLines();
        }
    }
}