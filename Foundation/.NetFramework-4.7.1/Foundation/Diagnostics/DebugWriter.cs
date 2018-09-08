using System.Diagnostics;
using System.IO;
using System.Text;

namespace Foundation.Diagnostics
{
    public class DebugWriter : TextWriter
    {
        private static DebugWriter instance;

        public static DebugWriter Instance
        {
            get
            {
                if (instance == null)
                    instance = new DebugWriter();

                return instance;
            }
        }

        public override Encoding Encoding => null;

        public override void Write(char[] buffer, int index, int count)
        {
            var message = new string(buffer, index, count);
            Debug.Write(message);
        }
    }
}