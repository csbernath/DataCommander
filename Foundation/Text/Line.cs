using Foundation.Assertions;
using Foundation.Core;

namespace Foundation.Text;

public sealed class Line
{
    public readonly int Indentation;
    public readonly string Text;

    public Line(int indentation, string text)
    {
        Assert.IsInRange(indentation >= 0);

        Indentation = indentation;
        Text = text;
    }

    public Line Indent(int indentation) =>
        Text.IsNullOrEmpty()
            ? this
            : new Line(Indentation + indentation, Text);

    public static Line Empty = new(0, string.Empty);
}