using System;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace Foundation.IO
{
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
            _path = path;
            _tempPath = tempPath;
            Writer = new StreamWriter(tempPath, false);
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
            _path = path;
            _tempPath = tempPath;
            Writer = new StreamWriter(tempPath, false, encoding);
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
            Writer.Close();
            const NativeMethods.MoveFileExFlags flags = NativeMethods.MoveFileExFlags.ReplaceExisiting;
            var succeeded = NativeMethods.MoveFileEx(_tempPath, _path, flags);

            if (!succeeded)
            {
                throw new Win32Exception();
            }

            _commited = true;
        }

        /// <summary>
        /// 
        /// </summary>
        void IDisposable.Dispose()
        {
            Writer.Dispose();

            if (!_commited)
            {
                File.Delete(_tempPath);
            }
        }
    }
}