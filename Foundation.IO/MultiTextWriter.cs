using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Foundation.IO
{
    /// <summary>
    /// 
    /// </summary>
    public class MultiTextWriter : TextWriter
    {
        private readonly List<TextWriter> _textWriters = new List<TextWriter>();

        /// <summary>
        /// 
        /// </summary>
        public IList<TextWriter> TextWriters => _textWriters;

        /// <summary>
        /// 
        /// </summary>
        public override Encoding Encoding => throw new NotImplementedException();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public override void Write(char value)
        {
            foreach (var textWriter in _textWriters)
            {
                textWriter.Write(value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        public override void Write(char[] buffer, int index, int count)
        {
            var value = new string(buffer, index, count);
            Write(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public override void Write(string value)
        {
            foreach (var textWriter in _textWriters)
            {
                textWriter.Write(value);
            }
        }
    }
}