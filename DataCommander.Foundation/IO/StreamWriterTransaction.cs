namespace DataCommander.Foundation.IO
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Text;

    /// <summary>
    /// 
    /// </summary>
    public sealed class StreamWriterTransaction : IDisposable
    {
        #region Private Fields

        private readonly StreamWriter streamWriter;
        private readonly String path;
        private readonly String tempPath;
        private Boolean commited;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="tempPath"></param>
        public StreamWriterTransaction(
            String path,
            String tempPath )
        {
            this.path = path;
            this.tempPath = tempPath;
            this.streamWriter = new StreamWriter( tempPath, false );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="tempPath"></param>
        /// <param name="encoding"></param>
        public StreamWriterTransaction(
            String path,
            String tempPath,
            Encoding encoding )
        {
            this.path = path;
            this.tempPath = tempPath;
            this.streamWriter = new StreamWriter( tempPath, false, encoding );
        }

        /// <summary>
        /// 
        /// </summary>
        public StreamWriter Writer
        {
            get
            {
                return this.streamWriter;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Commit()
        {
            this.streamWriter.Close();
            const NativeMethods.MoveFileExFlags flags = NativeMethods.MoveFileExFlags.ReplaceExisiting;
            Boolean succeeded = NativeMethods.MoveFileEx( this.tempPath, this.path, flags );

            if (!succeeded)
            {
                throw new Win32Exception();
            }

            this.commited = true;
        }

        /// <summary>
        /// 
        /// </summary>
        void IDisposable.Dispose()
        {
            this.streamWriter.Dispose();

            if (!this.commited)
            {
                File.Delete( this.tempPath );
            }
        }
    }
}