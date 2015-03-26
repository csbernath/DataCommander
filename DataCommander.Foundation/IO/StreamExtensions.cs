#if FOUNDATION_3_5

namespace DataCommander.Foundation.IO
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;

    /// <summary>
    /// 
    /// </summary>
    public static class StreamExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="outputStream"></param>
        public static void CopyTo( this Stream inputStream, Stream outputStream )
        {
            inputStream.CopyTo( outputStream, 81920 );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="outputStream"></param>
        /// <param name="bufferSize"></param>
        public static void CopyTo( this Stream inputStream, Stream outputStream, int bufferSize )
        {
            Contract.Requires<ArgumentNullException>( inputStream != null );
            Contract.Requires<ArgumentNullException>( outputStream != null );
            Contract.Requires<ArgumentOutOfRangeException>( bufferSize > 0 );

            var buffer = new Byte[bufferSize];

            while (true)
            {
                int read = inputStream.Read( buffer, 0, buffer.Length );
                if (read == 0)
                {
                    break;
                }

                outputStream.Write( buffer, 0, read );
            }
        }
    }
}

#endif