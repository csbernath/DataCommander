namespace DataCommander.Foundation.IO
{
    using System;
    using System.Globalization;
    using System.IO;

    /// <summary>
    /// 
    /// </summary>
    public sealed class TreeTextWriter
    {
        private readonly TextWriter textWriter;
        private readonly int indentation;
        private int level;
        private State state;
        private readonly ConsoleColor originalForegroundColor;
        private ConsoleColor foregroundColor;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textWriter"></param>
        /// <param name="indentation"></param>
        public TreeTextWriter( TextWriter textWriter, int indentation )
        {
            this.textWriter = textWriter;
            this.indentation = indentation;
            this.state = State.WriteEndElement;
            this.originalForegroundColor = Console.ForegroundColor;
            this.foregroundColor = this.originalForegroundColor;
        }

        /// <summary>
        /// 
        /// </summary>
        public ConsoleColor ForegroundColor
        {
            get
            {
                return this.foregroundColor;
            }

            set
            {
                this.foregroundColor = value;
            }
        }

        private enum State
        {
            WriteStartElement,
            WriteEndElement
        }

        private void WritePrefix( int level )
        {
            if (level > 0)
            {
                string prefix = '|' + new string( ' ', this.indentation - 1 );

                for (int i = 0;i < level;i++)
                {
                    this.textWriter.Write( prefix );
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void WriteStartElement( string value )
        {
            if (this.state == State.WriteStartElement)
            {
                this.textWriter.WriteLine();
            }

            this.WritePrefix( this.level );
            this.textWriter.Write( value );
            this.level++;
            this.state = State.WriteStartElement;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arguments"></param>
        public void WriteStartElement( string format, params object[] arguments )
        {
            string value = string.Format( CultureInfo.InvariantCulture, format, arguments );
            this.WriteStartElement( value );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void WriteEndElement( string value )
        {
            this.level--;

            if (this.state == State.WriteEndElement)
            {
                this.WritePrefix( this.level );
            }

            this.textWriter.WriteLine( value );

            if (this.state == State.WriteStartElement)
            {
            }

            this.state = State.WriteEndElement;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arguments"></param>
        public void WriteEndElement( string format, params object[] arguments )
        {
            string value = string.Format( CultureInfo.InvariantCulture, format, arguments );
            this.WriteEndElement( value );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void WriteElement( string value )
        {
            if (this.state == State.WriteStartElement)
            {
                this.textWriter.WriteLine();
            }

            this.WritePrefix( this.level );

            if (this.originalForegroundColor != this.foregroundColor)
            {
                Console.ForegroundColor = this.foregroundColor;
            }

            this.textWriter.WriteLine( value );

            if (this.originalForegroundColor != this.foregroundColor)
            {
                Console.ForegroundColor = this.originalForegroundColor;
            }

            this.state = State.WriteEndElement;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arguments"></param>
        public void WriteElement( string format, params object[] arguments )
        {
            string value = string.Format( CultureInfo.InvariantCulture, format, arguments );
            this.WriteElement( value );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void WriteElement( object value )
        {
            string s = value != null ? value.ToString() : null;
            this.WriteElement( s );
        }
    }
}