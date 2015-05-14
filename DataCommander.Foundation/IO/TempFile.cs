namespace DataCommander.Foundation.IO
{
    using System;
    using System.IO;

    /// <summary>
    /// 
    /// </summary>
    public sealed class TempFile : IDisposable
    {
        private string filename;
        private bool deleted;

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
            this.filename = filename;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Filename
        {
            get
            {
                if (this.filename == null)
                {
                    this.filename = Path.GetTempFileName();
                }

                return this.filename;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Delete()
        {
            this.deleted = true;
            File.Delete(this.filename);
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            if (this.filename != null && !this.deleted)
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