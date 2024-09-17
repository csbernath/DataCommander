using System.Collections.Generic;
using System.Collections.ObjectModel;
using Foundation.Text;

namespace Foundation.Data.SqlClient.DbQueryBuilding;

public static class DataTransferObjectWithPropertiesFactory
{
    public static ReadOnlyCollection<Line> Create(string name, IReadOnlyCollection<DataTransferObjectField> fields)
    {
        TextBuilder textBuilder = new TextBuilder();
        textBuilder.Add($"public class {name}");
        using (textBuilder.AddCSharpBlock())
            foreach (DataTransferObjectField field in fields)
                textBuilder.Add($"public {field.Type} {field.Name} {{ get; set; }}");
        return textBuilder.ToLines();
    }
}