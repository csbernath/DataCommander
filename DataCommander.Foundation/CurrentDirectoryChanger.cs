namespace DataCommander.Foundation
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public sealed class CurrentDirectoryChanger : IDisposable
    {
        private readonly String currentDirectory;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public CurrentDirectoryChanger( String path )
        {
            this.currentDirectory = Environment.CurrentDirectory;
            Environment.CurrentDirectory = path;
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            Environment.CurrentDirectory = this.currentDirectory;
        }

        #endregion
    }
}