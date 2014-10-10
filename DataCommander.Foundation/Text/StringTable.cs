namespace DataCommander.Foundation.Text
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Represents a n x m matrix of strings.
    /// </summary>
    public class StringTable
    {
        private readonly StringTableColumnCollection columns = new StringTableColumnCollection();
        private StringTableRowCollection rows = new StringTableRowCollection();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnCount"></param>
        public StringTable( Int32 columnCount )
        {
            Contract.Requires( columnCount >= 0 );

            for (Int32 i = 0; i < columnCount; i++)
            {
                this.columns.Add( new StringTableColumn() );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public StringTableColumnCollection Columns
        {
            get
            {
                return this.columns;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public StringTableRowCollection Rows
        {
            get
            {
                return this.rows;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public StringTableRow NewRow()
        {
            return new StringTableRow( this );
        }

        internal Int32 GetMaxColumnWidth( Int32 columnIndex )
        {
            Int32 rowCount = this.rows.Count;
            Int32 max = 0;

            for (Int32 i = 0; i < rowCount; i++)
            {
                String s = this.rows[ i ][ columnIndex ];
                Int32 width = s == null ? 0 : s.Length;

                if (width > max)
                {
                    max = width;
                }
            }

            return max;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            var sb = new StringBuilder();
            Int32 count = this.columns.Count;

            for (Int32 i = 0; i < count; i++)
            {
                Int32 maxColumnWidth = this.GetMaxColumnWidth( i );
                this.columns[ i ].Width = maxColumnWidth;
            }

            Int32 last = count - 1;

            for (Int32 i = 0; i < this.rows.Count; i++)
            {
                this.WriteRow( i, sb );
                sb.AppendLine();
            }

            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textWriter"></param>
        public void Write( TextWriter textWriter )
        {
            Contract.Requires( textWriter != null );

            for (Int32 i = 0; i < this.columns.Count; i++)
            {
                Int32 maxColumnWidth = this.GetMaxColumnWidth( i );
                this.columns[ i ].Width = maxColumnWidth;
            }

            for (Int32 i = 0; i < this.rows.Count; i++)
            {
                var sb = new StringBuilder();
                this.WriteRow( i, sb );
                textWriter.WriteLine( sb );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textWriter"></param>
        /// <param name="indent"></param>
        public void Write( TextWriter textWriter, Int32 indent )
        {
            Contract.Requires( textWriter != null );

            Int32 last = this.columns.Count - 1;

            for (Int32 i = 0; i <= last; i++)
            {
                Int32 width = this.GetMaxColumnWidth( i );

                if (i < last)
                {
                    Int32 remainder = (width + 1) % indent;

                    if (remainder != 0)
                    {
                        width += indent - remainder;
                    }
                }

                this.columns[ i ].Width = width;
            }

            for (Int32 i = 0; i < this.rows.Count; i++)
            {
                var sb = new StringBuilder();
                this.WriteRow( i, sb );
                textWriter.WriteLine( sb );
            }
        }

        private void WriteRow( Int32 rowIndex, StringBuilder sb )
        {
            StringTableRow row = this.rows[ rowIndex ];
            Int32 last = this.columns.Count - 1;

            for (Int32 j = 0; j <= last; j++)
            {
                StringTableColumn column = this.columns[ j ];
                Boolean alignRight = column.Align == StringTableColumnAlign.Right;
                if (j < last)
                {
                    String s = StringHelper.FormatColumn( row[ j ], column.Width, alignRight );
                    sb.Append( s );
                    sb.Append( ' ' );
                }
                else
                {
                    if (alignRight)
                    {
                        String s = StringHelper.FormatColumn( row[ j ], column.Width, alignRight );
                        sb.Append( s );
                    }
                    else
                    {
                        sb.Append( row[ j ] );
                    }
                }
            }
        }
    }
}