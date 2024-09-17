using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Foundation.Text;

namespace Foundation.Data.SqlClient.DbQueryBuilding;

public static class CreateDeleteSqlStatementMethodFactory
{
    public static ReadOnlyCollection<Line> Create(string schema, string table, Column identifierColumn, Column versionColumn)
    {
        List<string> arguments = [];
        string csharpTypeName = SqlDataTypeRepository.SqlDataTypes.First(i => i.SqlDataTypeName == identifierColumn.SqlDataTypeName).CSharpTypeName;
        arguments.Add($"{csharpTypeName} {identifierColumn.ColumnName.ToCamelCase()}");
        if (versionColumn != null)
        {
            csharpTypeName = SqlDataTypeRepository.SqlDataTypes.First(i => i.SqlDataTypeName == versionColumn.SqlDataTypeName).CSharpTypeName;
            arguments.Add($"{csharpTypeName} {versionColumn.ColumnName.ToCamelCase()}");
        }

        TextBuilder textBuilder = new TextBuilder();
        textBuilder.Add($"public static ReadOnlyCollection<Line> CreateDeleteSqlStatement({arguments.Join(", ")})");
        using (textBuilder.AddCSharpBlock())
        {
            textBuilder.Add("var whereColumns = new[]");
            using (textBuilder.AddCSharpBlock())
            {
                string method = MethodName.GetToSqlConstantMethodName(identifierColumn.SqlDataTypeName, identifierColumn.IsNullable);
                textBuilder.Add($"new ColumnNameValue(\"{identifierColumn.ColumnName}\", {identifierColumn.ColumnName.ToCamelCase()}.{method}())");
                if (versionColumn != null)
                {
                    textBuilder.AddToLastLine(",");
                    method = MethodName.GetToSqlConstantMethodName(versionColumn.SqlDataTypeName, versionColumn.IsNullable);
                    textBuilder.Add($"new ColumnNameValue(\"{versionColumn.ColumnName}\", {versionColumn.ColumnName.ToCamelCase()}.{method}())");
                }
            }

            textBuilder.AddToLastLine(";");
            textBuilder.Add($"var deleteSqlStatement = DeleteSqlStatementFactory.Create(\"{schema}.{table}\", whereColumns);");
            textBuilder.Add($"var validation = ValidationFactory.Create(\"delete {schema}.{table} failed\");");
            textBuilder.Add("var textBuilder = new TextBuilder();");
            textBuilder.Add("textBuilder.Add(deleteSqlStatement);");
            textBuilder.Add("textBuilder.Add(validation);");
            textBuilder.Add("return textBuilder.ToLines();");
        }

        return textBuilder.ToLines();
    }

    //private static string GetCSharpTypeName(Column column)
    //{
    //    var csharpTypeName = SqlDataTypeArray.SqlDataTypes.First(i => i.SqlDataTypeName == column.SqlDataTypeName).CSharpTypeName;
    //    var csharpType = CSharpTypeArray.CSharpTypes.First(i => i.Name == csharpTypeName);
    //    if (column.IsNullable && csharpType.Type.IsValueType)
    //        csharpTypeName += "?";
    //    return csharpTypeName;
    //}
}