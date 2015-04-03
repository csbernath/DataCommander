namespace DataCommander.Foundation.IO
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;

    /// <summary>
    /// 
    /// </summary>
    public class EventableStream : Stream
    {
        private readonly Stream stream;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        public EventableStream( Stream stream )
        {
            Contract.Requires<ArgumentNullException>( stream != null );
            this.stream = stream;
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool CanRead
        {
            get
            {
                return this.stream.CanRead;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool CanSeek
        {
            get
            {
                return this.stream.CanSeek;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool CanWrite
        {
            get
            {
                return this.stream.CanWrite;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Flush()
        {
            this.stream.Flush();
        }

        /// <summary>
        /// 
        /// </summary>
        public override long Length
        {
            get
            {
                return this.stream.Length;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override long Position
        {
            get
            {
                return this.stream.Position;
            }

            set
            {
                this.stream.Position = value;
            }
        }

        private EventHandler beforeRead;

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler BeforeRead
        {
            add
            {
                this.beforeRead += value;
            }

            remove
            {
                this.beforeRead -= value;
            }
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
            if (this.beforeRead != null)
            {
                this.beforeRead( this, null );
            }

            return this.stream.Read( buffer, offset, count );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public override long Seek( long offset, SeekOrigin origin )
        {
            return this.stream.Seek( offset, origin );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public override void SetLength( long value )
        {
            this.stream.SetLength( value );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public override void Write( byte[] buffer, int offset, int count )
        {
            this.stream.Write( buffer, offset, count );
        }

#if FOUNDATION_3_5
        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant( this.stream != null );
        }
#else
        [ContractInvariantMethod]
        private new void ObjectInvariant()
        {
            Contract.Invariant( this.stream != null );
        }
#endif
    }
}