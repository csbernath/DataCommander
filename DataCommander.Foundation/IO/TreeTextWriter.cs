using System;
using System.Globalization;
using System.IO;

namespace Foundation.IO
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class TreeTextWriter
    {
        private readonly TextWriter _textWriter;
        private readonly int _indentation;
        private int _level;
        private State _state;
        private readonly ConsoleColor _originalForegroundColor;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textWriter"></param>
        /// <param name="indentation"></param>
        public TreeTextWriter(TextWriter textWriter, int indentation)
        {
            this._textWriter = textWriter;
            this._indentation = indentation;
            this._state = State.WriteEndElement;
            this._originalForegroundColor = Console.ForegroundColor;
            this.ForegroundColor = this._originalForegroundColor;
        }

        /// <summary>
        /// 
        /// </summary>
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
                var prefix = '|' + new string(' ', this._indentation - 1);

                for (var i = 0; i < level; i++)
                {
                    this._textWriter.Write(prefix);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void WriteStartElement(string value)
        {
            if (this._state == State.WriteStartElement)
            {
                this._textWriter.WriteLine();
            }

            this.WritePrefix(this._level);
            this._textWriter.Write(value);
            this._level++;
            this._state = State.WriteStartElement;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arguments"></param>
        public void WriteStartElement(string format, params object[] arguments)
        {
            var value = string.Format(CultureInfo.InvariantCulture, format, arguments);
            this.WriteStartElement(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void WriteEndElement(string value)
        {
            this._level--;

            if (this._state == State.WriteEndElement)
            {
                this.WritePrefix(this._level);
            }

            this._textWriter.WriteLine(value);

            if (this._state == State.WriteStartElement)
            {
            }

            this._state = State.WriteEndElement;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arguments"></param>
        public void WriteEndElement(string format, params object[] arguments)
        {
            var value = string.Format(CultureInfo.InvariantCulture, format, arguments);
            this.WriteEndElement(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void WriteElement(string value)
        {
            if (this._state == State.WriteStartElement)
            {
                this._textWriter.WriteLine();
            }

            this.WritePrefix(this._level);

            if (this._originalForegroundColor != this.ForegroundColor)
            {
                Console.ForegroundColor = this.ForegroundColor;
            }

            this._textWriter.WriteLine(value);

            if (this._originalForegroundColor != this.ForegroundColor)
            {
                Console.ForegroundColor = this._originalForegroundColor;
            }

            this._state = State.WriteEndElement;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arguments"></param>
        public void WriteElement(string format, params object[] arguments)
        {
            var value = string.Format(CultureInfo.InvariantCulture, format, arguments);
            this.WriteElement(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void WriteElement(object value)
        {
            var s = value != null ? value.ToString() : null;
            this.WriteElement(s);
        }
    }
}