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
            this._filename = filename;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Filename
        {
            get
            {
                if (this._filename == null)
                {
                    this._filename = Path.GetTempFileName();
                }

                return this._filename;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Delete()
        {
            this._deleted = true;
            File.Delete(this._filename);
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            if (this._filename != null && !this._deleted)
            {
                try
                {
                    this.Delete();
                }
                catch
                {
                }
            }
        }

        #endregion
    }
}