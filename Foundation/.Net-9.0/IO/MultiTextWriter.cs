using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Foundation.IO;

public class MultiTextWriter : TextWriter
{
    private readonly List<TextWriter> _textWriters = [];

    public IList<TextWriter> TextWriters => _textWriters;

    public override Encoding Encoding => throw new NotImplementedException();

    public override void Write(char value)
    {
        foreach (TextWriter textWriter in _textWriters)
            textWriter.Write(value);
    }

    public override void Write(char[] buffer, int index, int count)
    {
        string value = new string(buffer, index, count);
        Write(value);
    }

    public override void Write(string value)
    {
        foreach (TextWriter textWriter in _textWriters)
        {
            textWriter.Write(value);
        }
    }
}