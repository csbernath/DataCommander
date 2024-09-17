using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Foundation.Assertions;
using Foundation.Collections.ReadOnly;
using Foundation.Core;

namespace Foundation.Text;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class TextBuilder
{
    private readonly List<Line> _lines = [];
    private int _indentation;

    public void Add(string text)
    {
        Line line = new Line(_indentation, text);
        _lines.Add(line);
    }

    public void Add(Line line)
    {
        ArgumentNullException.ThrowIfNull(line);

        Line modifiedLine = line.Indent(_indentation);
        _lines.Add(modifiedLine);
    }

    public void Add(IEnumerable<Line> lines)
    {
        ArgumentNullException.ThrowIfNull(lines);

        IEnumerable<Line> modifiedLines = lines.Select(line => line.Indent(_indentation));
        _lines.AddRange(modifiedLines);
    }

    public void AddToLastLine(string text)
    {
        Assert.IsValidOperation(_lines.Count > 0);
        int last = _lines.Count - 1;
        Line line = _lines[last];
        Line modifiedLine = new Line(line.Indentation, line.Text + text);
        _lines[last] = modifiedLine;
    }

    public IDisposable Indent(int indentation)
    {
        _indentation += indentation;
        return new Disposer(() => _indentation -= indentation);
    }

    public ReadOnlyCollection<Line> ToLines() => _lines.ToReadOnlyCollection();

    private string DebuggerDisplay => _lines.ToIndentedString("    ");
}