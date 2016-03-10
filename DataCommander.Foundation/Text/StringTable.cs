namespace DataCommander.Foundation.Text
{
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Represents a n x m matrix of strings.
    /// </summary>
    public class StringTable
    {
        private readonly StringTableColumnCollection columns = new StringTableColumnCollection();
        private readonly StringTableRowCollection rows = new StringTableRowCollection();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnCount"></param>
        public StringTable( int columnCount )
        {
            Contract.Requires( columnCount >= 0 );

            for (int i = 0; i < columnCount; i++)
            {
                this.columns.Add( new StringTableColumn() );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public StringTableColumnCollection Columns => this.columns;

        /// <summary>
        /// 
        /// </summary>
        public StringTableRowCollection Rows => this.rows;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public StringTableRow NewRow()
        {
            return new StringTableRow( this );
        }

        internal int GetMaxColumnWidth( int columnIndex )
        {
            int rowCount = this.rows.Count;
            int max = 0;

            for (int i = 0; i < rowCount; i++)
            {
                string s = this.rows[ i ][ columnIndex ];
                int width = s == null ? 0 : s.Length;

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
        public override string ToString()
        {
            var sb = new StringBuilder();
            int count = this.columns.Count;

            for (int i = 0; i < count; i++)
            {
                int maxColumnWidth = this.GetMaxColumnWidth( i );
                this.columns[ i ].Width = maxColumnWidth;
            }

            int last = count - 1;

            for (int i = 0; i < this.rows.Count; i++)
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

            for (int i = 0; i < this.columns.Count; i++)
            {
                int maxColumnWidth = this.GetMaxColumnWidth( i );
                this.columns[ i ].Width = maxColumnWidth;
            }

            for (int i = 0; i < this.rows.Count; i++)
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
        public void Write( TextWriter textWriter, int indent )
        {
            Contract.Requires( textWriter != null );

            int last = this.columns.Count - 1;

            for (int i = 0; i <= last; i++)
            {
                int width = this.GetMaxColumnWidth( i );

                if (i < last)
                {
                    int remainder = (width + 1) % indent;

                    if (remainder != 0)
                    {
                        width += indent - remainder;
                    }
                }

                this.columns[ i ].Width = width;
            }

            for (int i = 0; i < this.rows.Count; i++)
            {
                var sb = new StringBuilder();
                this.WriteRow( i, sb );
                textWriter.WriteLine( sb );
            }
        }

        private void WriteRow( int rowIndex, StringBuilder sb )
        {
            StringTableRow row = this.rows[ rowIndex ];
            int last = this.columns.Count - 1;

            for (int j = 0; j <= last; j++)
            {
                StringTableColumn column = this.columns[ j ];
                bool alignRight = column.Align == StringTableColumnAlign.Right;
                if (j < last)
                {
                    string s = StringHelper.FormatColumn( row[ j ], column.Width, alignRight );
                    sb.Append( s );
                    sb.Append( ' ' );
                }
                else
                {
                    if (alignRight)
                    {
                        string s = StringHelper.FormatColumn( row[ j ], column.Width, alignRight );
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