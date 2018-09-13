using System;

namespace Foundation.Core
{
    public sealed class CurrentDirectoryChanger : IDisposable
    {
        private readonly string _currentDirectory;

        public CurrentDirectoryChanger(string path)
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