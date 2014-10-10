namespace DataCommander.Foundation.IO
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using DataCommander.Foundation;
    using DataCommander.Foundation.Diagnostics;

    /// <summary>
    /// 
    /// </summary>
    public sealed class SegmentedStreamReader : Stream
    {
        private static readonly ILog log = LogFactory.Instance.GetCurrentTypeLog();
        private readonly Stream stream;
        private Int64 length;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="length"></param>
        public SegmentedStreamReader( Stream stream, Int64 length )
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
            GarbageMonitor.SetDisposeTime( this, OptimizedDateTime.Now );
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
        public override Int64 Seek( Int64 offset, SeekOrigin origin )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public override void SetLength( Int64 value )
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
        public override Int32 Read( Byte[] buffer, Int32 offset, Int32 count )
        {
            Contract.Assert( count >= 0 );

            Int32 read;
            Int64 position = this.stream.Position;

            if (position < this.length)
            {
                Int32 min = (Int32)Math.Min( this.length - position, count );
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
        public override void Write( Byte[] buffer, Int32 offset, Int32 count )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public override Boolean CanRead
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override Boolean CanSeek
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override Boolean CanWrite
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override Int64 Length
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override Int64 Position { get; set; }
    }
}