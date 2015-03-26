namespace DataCommander.Foundation.Diagnostics
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Summary description for TraceWriter.
    /// </summary>
    public class TraceWriter : TextWriter
    {
        private static TraceWriter instance;

        /// <summary>
        /// 
        /// </summary>
        public static TraceWriter Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TraceWriter();
                }

                return instance;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override Encoding Encoding
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        public override void Write(Char c)
        {
            Trace.Write(c);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        public override void Write(string str)
        {
            Trace.Write(str);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        public override void WriteLine(string str)
        {
            Trace.WriteLine(str);
        }
    }
}