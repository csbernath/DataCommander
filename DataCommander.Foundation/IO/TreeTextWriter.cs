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
        private readonly Int32 indentation;
        private Int32 level;
        private State state;
        private readonly ConsoleColor originalForegroundColor;
        private ConsoleColor foregroundColor;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textWriter"></param>
        /// <param name="indentation"></param>
        public TreeTextWriter( TextWriter textWriter, Int32 indentation )
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

        private void WritePrefix( Int32 level )
        {
            if (level > 0)
            {
                String prefix = '|' + new String( ' ', this.indentation - 1 );

                for (Int32 i = 0;i < level;i++)
                {
                    this.textWriter.Write( prefix );
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void WriteStartElement( String value )
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
        public void WriteStartElement( String format, params Object[] arguments )
        {
            String value = String.Format( CultureInfo.InvariantCulture, format, arguments );
            this.WriteStartElement( value );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void WriteEndElement( String value )
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
        public void WriteEndElement( String format, params Object[] arguments )
        {
            String value = String.Format( CultureInfo.InvariantCulture, format, arguments );
            this.WriteEndElement( value );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void WriteElement( String value )
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
        public void WriteElement( String format, params Object[] arguments )
        {
            String value = String.Format( CultureInfo.InvariantCulture, format, arguments );
            this.WriteElement( value );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void WriteElement( Object value )
        {
            String s = value != null ? value.ToString() : null;
            this.WriteElement( s );
        }
    }
}