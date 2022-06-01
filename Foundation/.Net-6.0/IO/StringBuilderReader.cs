using System;
using System.IO;
using System.Text;

namespace Foundation.IO;

public sealed class StringBuilderReader : TextReader
{
    private readonly StringBuilder _stringBuilder;
    private int _index;

    public StringBuilderReader(StringBuilder stringBuilder)
    {
        ArgumentNullException.ThrowIfNull(stringBuilder, nameof(stringBuilder));
        _stringBuilder = stringBuilder;
    }

    public override int Peek()
    {
        var result = _index < _stringBuilder.Length
            ? _stringBuilder[_index]
            : -1;
        return result;
    }

    public override int Read()
    {
        int result;

        if (_index < _stringBuilder.Length)
        {
            result = _stringBuilder[_index];
            ++_index;
        }
        else
            result = -1;

        return result;
    }

    public override int Read(char[] buffer, int index, int count)
    {
        var result = Math.Min(count, _stringBuilder.Length - _index);

        if (result > 0)
        {
            _stringBuilder.CopyTo(_index, buffer, index, result);
            _index += result;
        }

        return result;
    }
}