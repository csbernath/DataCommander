using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Foundation.Assertions;
using Foundation.Collections.ReadOnly;
using Foundation.Core;
using Foundation.Linq;

namespace Foundation.Text
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public sealed class TextBuilder
    {
        private readonly List<Line> _lines = new List<Line>();
        private int _indentation;

        public void Add(string text)
        {
            var line = new Line(_indentation, text);
            _lines.Add(line);
        }

        public void Add(Line line)
        {
            Assert.IsNotNull(line);

            var modifiedLine = line.Indent(_indentation);
            _lines.Add(modifiedLine);
        }

        public void Add(IEnumerable<Line> lines)
        {
            Assert.IsNotNull(lines);

            var modifiedLines = lines.Select(line => line.Indent(_indentation));
            _lines.AddRange(modifiedLines);
        }

        public void AddToLastLine(string text)
        {
            Assert.IsValidOperation(_lines.Count > 0);

            var line = _lines.Last();
            line = new Line(line.Indentation, line.Text + text);
        }

        public IDisposable Indent(int indentation)
        {
            _indentation += indentation;
            return new Disposer(() => _indentation -= indentation);
        }

        public ReadOnlyCollection<Line> ToReadOnlyCollection() => _lines.ToReadOnlyCollection();

        private string DebuggerDisplay => _lines.ToString("    ");
    }
}