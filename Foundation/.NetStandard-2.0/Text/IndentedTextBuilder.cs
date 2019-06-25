using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Foundation.Core;

namespace Foundation.Text
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public sealed class IndentedTextBuilder
    {
        private readonly List<IndentedLine> _lines = new List<IndentedLine>();
        private int _indentation;

        public void Add() => _lines.Add(new IndentedLine(0, string.Empty));

        public void Add(string text)
        {
            var indentedLine = new IndentedLine(_indentation, text);
            _lines.Add(indentedLine);
        }

        public void Add(IEnumerable<IndentedLine> lines)
        {
            var collection = lines.Select(line => new IndentedLine(_indentation + line.Indentation, line.Text));
            _lines.AddRange(collection);
        }

        public IDisposable Indent(int indentation)
        {
            _indentation += indentation;
            return new Disposer(() => _indentation -= indentation);
        }

        public ReadOnlyCollection<IndentedLine> ToReadOnlyCollection() => _lines.AsReadOnly();

        private string DebuggerDisplay => _lines.ToString("    ");
    }
}