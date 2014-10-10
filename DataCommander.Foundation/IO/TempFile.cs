namespace DataCommander.Foundation.IO
{
    using System;
    using System.IO;

    /// <summary>
    /// 
    /// </summary>
    public sealed class TempFile : IDisposable
    {
        private String filename;
        private Boolean deleted;

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
        public TempFile( String filename )
        {
            this.filename = filename;
        }

        /// <summary>
        /// 
        /// </summary>
        public String Filename
        {
            get
            {
                if (filename == null)
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
            File.Delete( this.filename );
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