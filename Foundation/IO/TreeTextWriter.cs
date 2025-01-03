using System;
using System.Globalization;
using System.IO;

namespace Foundation.IO;

public sealed class TreeTextWriter
{
    private readonly TextWriter _textWriter;
    private readonly int _indentation;
    private int _level;
    private State _state;
    private readonly ConsoleColor _originalForegroundColor;

    public TreeTextWriter(TextWriter textWriter, int indentation)
    {
        _textWriter = textWriter;
        _indentation = indentation;
        _state = State.WriteEndElement;
        _originalForegroundColor = Console.ForegroundColor;
        ForegroundColor = _originalForegroundColor;
    }

    public ConsoleColor ForegroundColor { get; set; }

    private enum State
    {
        WriteStartElement,
        WriteEndElement
    }

    private void WritePrefix(int level)
    {
        if (level > 0)
        {
            var prefix = '|' + new string(' ', _indentation - 1);

            for (var i = 0; i < level; ++i)
                _textWriter.Write(prefix);
        }
    }

    public void WriteStartElement(string value)
    {
        if (_state == State.WriteStartElement)
            _textWriter.WriteLine();

        WritePrefix(_level);
        _textWriter.Write(value);
        ++_level;
        _state = State.WriteStartElement;
    }

    public void WriteStartElement(string format, params object[] arguments)
    {
        var value = string.Format(CultureInfo.InvariantCulture, format, arguments);
        WriteStartElement(value);
    }

    public void WriteEndElement(string value)
    {
        --_level;

        if (_state == State.WriteEndElement)
            WritePrefix(_level);

        _textWriter.WriteLine(value);

        if (_state == State.WriteStartElement)
        {
        }

        _state = State.WriteEndElement;
    }

    public void WriteEndElement(string format, params object[] arguments)
    {
        var value = string.Format(CultureInfo.InvariantCulture, format, arguments);
        WriteEndElement(value);
    }

    public void WriteElement(string value)
    {
        if (_state == State.WriteStartElement)
            _textWriter.WriteLine();

        WritePrefix(_level);

        if (_originalForegroundColor != ForegroundColor)
            Console.ForegroundColor = ForegroundColor;

        _textWriter.WriteLine(value);

        if (_originalForegroundColor != ForegroundColor)
            Console.ForegroundColor = _originalForegroundColor;

        _state = State.WriteEndElement;
    }

    public void WriteElement(string format, params object[] arguments)
    {
        var value = string.Format(CultureInfo.InvariantCulture, format, arguments);
        WriteElement(value);
    }

    public void WriteElement(object? value)
    {
        var s = value != null ? value.ToString() : null;
        WriteElement(s!);
    }
}