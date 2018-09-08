using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Foundation.IO
{
    public class MultiTextWriter : TextWriter
    {
        private readonly List<TextWriter> _textWriters = new List<TextWriter>();

        public IList<TextWriter> TextWriters => _textWriters;

        public override Encoding Encoding => throw new NotImplementedException();

        public override void Write(char value)
        {
            foreach (var textWriter in _textWriters)
                textWriter.Write(value);
        }

        public override void Write(char[] buffer, int index, int count)
        {
            var value = new string(buffer, index, count);
            Write(value);
        }

        public override void Write(string value)
        {
            foreach (var textWriter in _textWriters)
            {
                textWriter.Write(value);
            }
        }
    }
}