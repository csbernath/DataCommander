namespace DataCommander.Foundation
{
    using System;
    using System.Text;

    /// <exclude/>
    public sealed class Library
    {
        private readonly Int32 moduleHandle;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        public Library( String fileName )
        {
            this.moduleHandle = NativeMethods.LoadLibrary( fileName );
        }

        /// <summary>
        /// 
        /// </summary>
        ~Library()
        {
            if (this.moduleHandle != 0)
            {
                NativeMethods.FreeLibrary( this.moduleHandle );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public String LoadString( Int32 id )
        {
            byte[] buffer = new byte[ 1024 ];
            Int32 n = NativeMethods.LoadString( this.moduleHandle, id, buffer, buffer.Length );
            String value = null;

            if (n > 0)
            {
                char[] chars = Encoding.Default.GetChars( buffer, 0, n );
                value = new String( chars );
            }

            return value;
        }
    }
}