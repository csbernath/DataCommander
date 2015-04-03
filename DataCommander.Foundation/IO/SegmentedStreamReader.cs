namespace DataCommander.Foundation.IO
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using DataCommander.Foundation.Diagnostics;

    /// <summary>
    /// 
    /// </summary>
    public sealed class SegmentedStreamReader : Stream
    {
        private static readonly ILog log = LogFactory.Instance.GetCurrentTypeLog();
        private readonly Stream stream;
        private long length;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="length"></param>
        public SegmentedStreamReader( Stream stream, long length )
        {
            Contract.Requires( stream != null );

            this.stream = stream;
            this.length = length;
            GarbageMonitor.Add( null, "SegmentedStreamReader", 0, this );
            log.Trace( GarbageMonitor.State );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose( bool disposing )
        {
            base.Dispose( disposing );
            GarbageMonitor.SetDisposeTime( this, LocalTime.Default.Now );
            log.Trace( GarbageMonitor.State );
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Flush()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public override long Seek( long offset, SeekOrigin origin )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public override void SetLength( long value )
        {
            this.length = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override int Read( byte[] buffer, int offset, int count )
        {
            Contract.Assert( count >= 0 );

            int read;
            long position = this.stream.Position;

            if (position < this.length)
            {
                int min = (int)Math.Min( this.length - position, count );
                read = this.stream.Read( buffer, offset, min );
            }
            else
            {
                read = 0;
            }

            return read;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public override void Write( byte[] buffer, int offset, int count )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool CanRead
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool CanSeek
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool CanWrite
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override long Length
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override long Position { get; set; }
    }
}