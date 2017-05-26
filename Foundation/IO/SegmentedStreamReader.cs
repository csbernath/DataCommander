using System;
using System.IO;
using Foundation.Diagnostics;
using Foundation.Diagnostics.Log;

namespace Foundation.IO
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class SegmentedStreamReader : Stream
    {
        private static readonly ILog Log = LogFactory.Instance.GetCurrentTypeLog();
        private readonly Stream _stream;
        private long _length;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="length"></param>
        public SegmentedStreamReader(Stream stream, long length)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(stream != null);
#endif

            this._stream = stream;
            this._length = length;
            GarbageMonitor.Add(null, "SegmentedStreamReader", 0, this);
            Log.Trace(GarbageMonitor.State);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            GarbageMonitor.SetDisposeTime(this, LocalTime.Default.Now);
            Log.Trace(GarbageMonitor.State);
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
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public override void SetLength(long value)
        {
            this._length = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
#if CONTRACTS_FULL
            Contract.Assert(count >= 0);
#endif

            int read;
            var position = this._stream.Position;

            if (position < this._length)
            {
                var min = (int) Math.Min(this._length - position, count);
                read = this._stream.Read(buffer, offset, min);
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
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool CanRead => throw new NotImplementedException();

        /// <summary>
        /// 
        /// </summary>
        public override bool CanSeek => throw new NotImplementedException();

        /// <summary>
        /// 
        /// </summary>
        public override bool CanWrite => throw new NotImplementedException();

        /// <summary>
        /// 
        /// </summary>
        public override long Length => throw new NotImplementedException();

        /// <summary>
        /// 
        /// </summary>
        public override long Position
        {
            get;
            set;
        }
    }
}