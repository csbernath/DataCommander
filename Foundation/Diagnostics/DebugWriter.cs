using System.Diagnostics;
using System.IO;
using System.Text;

namespace Foundation.Diagnostics;

public class DebugWriter : TextWriter
{
    private static DebugWriter? _instance;

    public static DebugWriter Instance
    {
        get
        {
            if (_instance == null)
                _instance = new DebugWriter();

            return _instance;
        }
    }

    public override Encoding Encoding => Encoding.Default;

    public override void Write(char[] buffer, int index, int count)
    {
        var message = new string(buffer, index, count);
        Debug.Write(message);
    }
}