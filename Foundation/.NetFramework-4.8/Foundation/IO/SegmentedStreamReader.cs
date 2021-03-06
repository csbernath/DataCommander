﻿using System;
using System.IO;
using Foundation.Assertions;
using Foundation.Diagnostics;
using Foundation.Log;

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
            Assert.IsNotNull(stream);

            _stream = stream;
            _length = length;
            GarbageMonitor.Default.Add(null, "SegmentedStreamReader", 0, this);
            Log.Trace(GarbageMonitor.Default.State);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            GarbageMonitor.Default.SetDisposeTime(this, LocalTime.Default.Now);
            Log.Trace(GarbageMonitor.Default.State);
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
            _length = value;
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

            Assert.IsTrue(count >= 0);

            int read;
            var position = _stream.Position;

            if (position < _length)
            {
                var min = (int) Math.Min(_length - position, count);
                read = _stream.Read(buffer, offset, min);
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