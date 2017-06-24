using System;

namespace Foundation
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class CurrentDirectoryChanger : IDisposable
    {
        private readonly string currentDirectory;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public CurrentDirectoryChanger( string path )
        {
            currentDirectory = Environment.CurrentDirectory;
            Environment.CurrentDirectory = path;
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            Environment.CurrentDirectory = currentDirectory;
        }

        #endregion
    }
}