namespace DataCommander.Foundation.Diagnostics
{
    using System;
    using System.IO;
    using System.Text;

    internal sealed class LogFile : ILogFile
    {
        private static readonly ILog log = InternalLogFactory.Instance.GetCurrentTypeLog();
        private String path;
        private String fileName;
        private DateTime date;
        private FileStream fileStream;
        private readonly Encoding encoding;
        private Int32 bufferSize;
        private Boolean autoFlush;
        private ILogFormatter formatter;
        private FileAttributes fileAttributes;

        public LogFile(
            String path,
            Encoding encoding,
            Int32 bufferSize,
            Boolean autoFlush,
            ILogFormatter formatter,
            FileAttributes fileAttributes)
        {
            this.path = path;
            this.encoding = encoding;
            this.bufferSize = bufferSize;
            this.autoFlush = autoFlush;
            this.formatter = formatter;
            this.fileAttributes = fileAttributes;
        }

        private static String GetNextFileName( String path, out DateTime date )
        {
            date = DateTime.Today;
            var sb = new StringBuilder();
            String directoryName = Path.GetDirectoryName( path );

            if (directoryName.Length > 0)
            {
                sb.Append( directoryName );
                sb.Append( Path.DirectorySeparatorChar );
            }

            sb.Append( Path.GetFileNameWithoutExtension( path ) );
            sb.Append( ' ' );
            sb.Append( date.ToString( "yyyy.MM.dd" ) );
            sb.Append( " [{0}]" );
            sb.Append( Path.GetExtension( path ) );
            String format = sb.ToString();
            Int32 id = 0;
            String idStr = null;

            while (true)
            {
                idStr = id.ToString().PadLeft( 2, '0' );
                String fileName = String.Format( format, idStr );
                Boolean exists = File.Exists( fileName );

                if (!exists)
                {
                    break;
                }

                id++;
            }

            return String.Format( format, idStr );
        }

        private void Open()
        {
            this.fileName = GetNextFileName( this.path, out this.date );

            try
            {
                this.fileStream = new FileStream( this.fileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read, this.bufferSize );
            }
            catch (Exception e)
            {
                log.Write( LogLevel.Error, e.ToString() );
                String directory = Path.GetTempPath();
                this.fileName = Path.GetFileName( this.path );
                StringBuilder sb = new StringBuilder();
                sb.Append( directory );
                sb.Append( this.fileName );
                this.path = sb.ToString();
                this.fileName = GetNextFileName( this.path, out this.date );
                this.fileStream = new FileStream( this.fileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read, this.bufferSize, true );
                log.Write( LogLevel.Error, String.Format( "LogFile path: {0}", this.fileName ) );
            }

            if (this.fileStream.Length == 0)
            {
                Byte[] preamble = this.encoding.GetPreamble();
                this.fileStream.Write( preamble, 0, preamble.Length );
            }
        }

        public void Write( DateTime date, String text )
        {
            if (this.fileStream == null)
            {
                this.Open();
            }
            else if (date != this.date)
            {
                this.Close();
                this.Open();
            }

            Byte[] array = this.encoding.GetBytes( text );
            this.fileStream.Write( array, 0, array.Length );

            if (this.autoFlush)
            {
                this.fileStream.Flush();
            }
        }

        #region ILogFile Members

        void ILogFile.Open()
        {
            String begin = this.formatter.Begin();

            if (begin != null)
            {
                this.Write( OptimizedDateTime.Now, begin );
            }
        }

        void ILogFile.Write( LogEntry entry )
        {
            String text = this.formatter.Format( entry );
            this.Write( entry.CreationTime.Date, text );
        }

        void ILogFile.Flush()
        {
            this.fileStream.Flush();
        }

        public void Close()
        {
            if (this.fileStream != null)
            {
                String end = this.formatter.End();

                if (end != null)
                {
                    this.Write( DateTime.Today, end );
                }

                this.fileStream.Close();
                String name = this.fileStream.Name;
                this.fileStream = null;

                if (this.fileAttributes != default( FileAttributes ))
                {
                    FileAttributes attributes = File.GetAttributes( name );
                    attributes |= this.fileAttributes; // FileAttributes.ReadOnly | FileAttributes.Hidden;
                    File.SetAttributes( name, attributes );
                }
            }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            this.Close();
        }

        #endregion
    }
}