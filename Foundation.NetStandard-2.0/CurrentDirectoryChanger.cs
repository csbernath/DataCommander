using System;

namespace Foundation
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class CurrentDirectoryChanger : IDisposable
    {
        private readonly string _currentDirectory;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public CurrentDirectoryChanger( string path )
        {
            _currentDirectory = Environment.CurrentDirectory;
            Environment.CurrentDirectory = path;
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            Environment.CurrentDirectory = _currentDirectory;
        }

        #endregion
    }
}