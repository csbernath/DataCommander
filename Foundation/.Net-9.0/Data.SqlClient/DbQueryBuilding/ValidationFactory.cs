using System.Collections.ObjectModel;
using Foundation.Text;

namespace Foundation.Data.SqlClient.DbQueryBuilding;

public static class ValidationFactory
{
    public static ReadOnlyCollection<Line> Create(string message)
    {
        var textBuilder = new TextBuilder();
        textBuilder.Add("textBuilder.Add(\"if @@rowcount = 0\");");
        textBuilder.Add("using (textBuilder.AddBlock(\"begin\", \"end\"))");
        using (textBuilder.AddCSharpBlock())
        {
            textBuilder.Add($"textBuilder.Add(\"raiserror('{message}',16,1)\");");
            textBuilder.Add("textBuilder.Add(\"return\");");
        }

        return textBuilder.ToLines();
    }
}