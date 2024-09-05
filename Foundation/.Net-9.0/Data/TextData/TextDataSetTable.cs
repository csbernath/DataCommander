using Foundation.Assertions;

namespace Foundation.Data.TextData;

public sealed class TextDataSetTable
{
    public TextDataSetTable(string name, int rowCount, TextDataTable table)
    {
        Assert.IsValidOperation(rowCount >= 0);

        Name = name;
        RowCount = rowCount;
        Table = table;
    }

    public string Name { get; }

    public int RowCount { get; }

    public TextDataTable Table { get; }
}