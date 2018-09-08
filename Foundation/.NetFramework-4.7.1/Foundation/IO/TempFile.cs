using System;
using System.IO;

namespace Foundation.IO
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class TempFile : IDisposable
    {
        private string _filename;
        private bool _deleted;

        /// <summary>
        /// 
        /// </summary>
        public TempFile()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        public TempFile(string filename)
        {
            _filename = filename;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Filename
        {
            get
            {
                if (_filename == null)
                {
                    _filename = Path.GetTempFileName();
                }

                return _filename;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Delete()
        {
            _deleted = true;
            File.Delete(_filename);
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            if (_filename != null && !_deleted)
            {
                try
                {
                    Delete();
                }
                catch
                {
                }
            }
        }

        #endregion
    }
}