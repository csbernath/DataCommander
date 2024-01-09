using System.Diagnostics;

namespace Foundation.Data.TextData;

[DebuggerDisplay("Name = {" + nameof(Name) + "}")]
public sealed class TextDataTable(string name)
{
    public string Name { get; } = name;

    public TextDataColumnCollection Columns { get; } = [];
}