using System.Collections.ObjectModel;
using System.Linq;
using Foundation.Text;

namespace Foundation.Data.SqlClient.DbQueryBuilding;

public static class DataTransferObjectFactory
{
    public static ReadOnlyCollection<Line> CreateDataTransferObject(string name, ReadOnlyCollection<DataTransferObjectField> fields)
    {
        TextBuilder textBuilder = new TextBuilder();

        textBuilder.Add($"public sealed class {name}");
        using (textBuilder.AddCSharpBlock())
        {
            foreach (DataTransferObjectField field in fields)
                textBuilder.Add($"public readonly {field.Type} {field.Name};");

            textBuilder.Add(Line.Empty);
            ReadOnlyCollection<Line> constructor = GetConstructor(name, fields);
            textBuilder.Add(constructor);
        }

        return textBuilder.ToLines();
    }

    private static ReadOnlyCollection<Line> GetConstructor(string name, ReadOnlyCollection<DataTransferObjectField> fields)
    {
        TextBuilder textBuilder = new TextBuilder();

        string parameters = fields.Select(field => $"{field.Type} {field.Name.ToCamelCase()}").Join(", ");
        textBuilder.Add($"public {name}({parameters})");
        using (textBuilder.AddCSharpBlock())
        {
            foreach (DataTransferObjectField field in fields)
                textBuilder.Add($"{field.Name} = {field.Name.ToCamelCase()};");
        }

        return textBuilder.ToLines();
    }
}