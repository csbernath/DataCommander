namespace DataCommander.Foundation.IO
{
    using System;
    using System.IO;

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
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(stream != null);
#endif
            this._stream = stream;
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool CanRead => this._stream.CanRead;

        /// <summary>
        /// 
        /// </summary>
        public override bool CanSeek => this._stream.CanSeek;

        /// <summary>
        /// 
        /// </summary>
        public override bool CanWrite => this._stream.CanWrite;

        /// <summary>
        /// 
        /// </summary>
        public override void Flush()
        {
            this._stream.Flush();
        }

        /// <summary>
        /// 
        /// </summary>
        public override long Length => this._stream.Length;

        /// <summary>
        /// 
        /// </summary>
        public override long Position
        {
            get => this._stream.Position;

            set => this._stream.Position = value;
        }

        private EventHandler _beforeRead;

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler BeforeRead
        {
            add => this._beforeRead += value;

            remove => this._beforeRead -= value;
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
            if (this._beforeRead != null)
            {
                this._beforeRead(this, null);
            }

            return this._stream.Read(buffer, offset, count);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            return this._stream.Seek(offset, origin);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public override void SetLength(long value)
        {
            this._stream.SetLength(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            this._stream.Write(buffer, offset, count);
        }

#if FOUNDATION_3_5
        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant( this.stream != null );
        }
#else
        //[ContractInvariantMethod]
        private new void ObjectInvariant()
        {
#if CONTRACTS_FULL
            Contract.Invariant(this.stream != null);
#endif
        }
#endif
        }
    }