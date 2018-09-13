using System;
using System.IO;
using Foundation.Assertions;
using Foundation.Core;
using Foundation.Diagnostics;
using Foundation.Log;

namespace Foundation.IO
{
    public sealed class SegmentedStreamReader : Stream
    {
        private static readonly ILog Log = LogFactory.Instance.GetCurrentTypeLog();
        private readonly Stream _stream;
        private long _length;

        public SegmentedStreamReader(Stream stream, long length)
        {
            Assert.IsNotNull(stream);

            _stream = stream;
            _length = length;
            GarbageMonitor.Default.Add(null, "SegmentedStreamReader", 0, this);
            Log.Trace(GarbageMonitor.Default.State);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            GarbageMonitor.Default.SetDisposeTime(this, LocalTime.Default.Now);
            Log.Trace(GarbageMonitor.Default.State);
        }

        public override void Flush() => throw new NotImplementedException();
        public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();
        public override void SetLength(long value) => _length = value;

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
                read = 0;

            return read;
        }

        public override void Write(byte[] buffer, int offset, int count) => throw new NotImplementedException();
        public override bool CanRead => throw new NotImplementedException();
        public override bool CanSeek => throw new NotImplementedException();
        public override bool CanWrite => throw new NotImplementedException();
        public override long Length => throw new NotImplementedException();
        public override long Position { get; set; }
    }
}