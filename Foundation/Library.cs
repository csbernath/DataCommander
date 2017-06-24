using System.Text;

namespace Foundation
{
    /// <exclude/>
    public sealed class Library
    {
        private readonly int moduleHandle;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        public Library( string fileName )
        {
            moduleHandle = NativeMethods.LoadLibrary( fileName );
        }

        /// <summary>
        /// 
        /// </summary>
        ~Library()
        {
            if (moduleHandle != 0)
            {
                NativeMethods.FreeLibrary(moduleHandle);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string LoadString( int id )
        {
            var buffer = new byte[ 1024 ];
            var n = NativeMethods.LoadString(moduleHandle, id, buffer, buffer.Length );
            string value = null;

            if (n > 0)
            {
                var chars = Encoding.Default.GetChars( buffer, 0, n );
                value = new string( chars );
            }

            return value;
        }
    }
}