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

        private readonly string _path;
        private readonly string _tempPath;
        private bool _commited;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="tempPath"></param>
        public StreamWriterTransaction(
            string path,
            string tempPath)
        {
            this._path = path;
            this._tempPath = tempPath;
            this.Writer = new StreamWriter(tempPath, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="tempPath"></param>
        /// <param name="encoding"></param>
        public StreamWriterTransaction(
            string path,
            string tempPath,
            Encoding encoding)
        {
            this._path = path;
            this._tempPath = tempPath;
            this.Writer = new StreamWriter(tempPath, false, encoding);
        }

        /// <summary>
        /// 
        /// </summary>
        public StreamWriter Writer { get; }

        /// <summary>
        /// 
        /// </summary>
        public void Commit()
        {
            this.Writer.Close();
            const NativeMethods.MoveFileExFlags flags = NativeMethods.MoveFileExFlags.ReplaceExisiting;
            var succeeded = NativeMethods.MoveFileEx(this._tempPath, this._path, flags);

            if (!succeeded)
            {
                throw new Win32Exception();
            }

            this._commited = true;
        }

        /// <summary>
        /// 
        /// </summary>
        void IDisposable.Dispose()
        {
            this.Writer.Dispose();

            if (!this._commited)
            {
                File.Delete(this._tempPath);
            }
        }
    }
}