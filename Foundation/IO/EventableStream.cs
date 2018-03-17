using System;
using System.IO;
using Foundation.Diagnostics;
using Foundation.Diagnostics.Assertions;

namespace Foundation.IO
{
    /// <summary>
    /// 
    /// </summary>
    public class EventableStream : Stream
    {
        private readonly Stream _stream;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        public EventableStream(Stream stream)
        {
            Assert.IsNotNull(stream);

            _stream = stream;
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool CanRead => _stream.CanRead;

        /// <summary>
        /// 
        /// </summary>
        public override bool CanSeek => _stream.CanSeek;

        /// <summary>
        /// 
        /// </summary>
        public override bool CanWrite => _stream.CanWrite;

        /// <summary>
        /// 
        /// </summary>
        public override void Flush()
        {
            _stream.Flush();
        }

        /// <summary>
        /// 
        /// </summary>
        public override long Length => _stream.Length;

        /// <summary>
        /// 
        /// </summary>
        public override long Position
        {
            get => _stream.Position;

            set => _stream.Position = value;
        }

        private EventHandler _beforeRead;

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler BeforeRead
        {
            add => _beforeRead += value;

            remove => _beforeRead -= value;
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
            if (_beforeRead != null)
            {
                _beforeRead(this, null);
            }

            return _stream.Read(buffer, offset, count);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            return _stream.Seek(offset, origin);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public override void SetLength(long value)
        {
            _stream.SetLength(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            _stream.Write(buffer, offset, count);
        }

        //[ContractInvariantMethod]
        private new void ObjectInvariant()
        {
            //Contract.Invariant(this.stream != null);
        }
    }
}