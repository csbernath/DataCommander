using System.Collections.Generic;
using Foundation.Text;

namespace Foundation.Data.SqlClient.DbQueryBuilding;

public static class DataTransferObjectRecordFactory
{
    public static IReadOnlyCollection<Line> Create(string name, IReadOnlyCollection<DataTransferObjectField> fields)
    {
        var textBuilder = new TextBuilder();
        textBuilder.Add($"public record {name}");
        using (textBuilder.AddBlock("(", ");"))
        {
            var first = true;
            foreach (var field in fields)
            {
                if (first)
                    first = false;
                else
                    textBuilder.AddToLastLine(",");

                textBuilder.Add($"{field.GetTypeName()} {field.Name}");
            }
        }

        return textBuilder.ToLines();
    }
}