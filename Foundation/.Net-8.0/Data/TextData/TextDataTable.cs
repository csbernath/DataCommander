using System.Diagnostics;

namespace Foundation.Data.TextData;

[DebuggerDisplay("Name = {" + nameof(Name) + "}")]
public sealed class TextDataTable
{
    public TextDataTable(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public TextDataColumnCollection Columns { get; } = [];
}