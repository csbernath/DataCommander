namespace DataCommander.Foundation.IO
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    /// <summary>
    /// 
    /// </summary>
    public class MultiTextWriter : TextWriter
    {
        private readonly List<TextWriter> textWriters = new List<TextWriter>();

        /// <summary>
        /// 
        /// </summary>
        public IList<TextWriter> TextWriters => this.textWriters;

        /// <summary>
        /// 
        /// </summary>
        public override Encoding Encoding
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public override void Write(char value)
        {
            foreach (var textWriter in this.textWriters)
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
        public override void Write(Char[] buffer, int index, int count)
        {
            var value = new string(buffer, index, count);
            this.Write(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public override void Write(string value)
        {
            foreach (var textWriter in this.textWriters)
            {
                textWriter.Write(value);
            }
        }
    }
}