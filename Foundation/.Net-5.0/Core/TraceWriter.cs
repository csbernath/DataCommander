using System.Diagnostics;
using System.IO;
using System.Text;

namespace Foundation.Core;

public class TraceWriter : TextWriter
{
    private static TraceWriter _instance;

    public static TraceWriter Instance
    {
        get
        {
            if (_instance == null)
                _instance = new TraceWriter();
            return _instance;
        }
    }

    public override Encoding Encoding => null;
    public override void Write(char c) => Trace.Write(c);
    public override void Write(string str) => Trace.Write(str);
    public override void WriteLine(string str) => Trace.WriteLine(str);
}