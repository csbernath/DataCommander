using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace DataCommander.Application;

public class TextBoxWriter(TextBoxBase textBox) : TextWriter
{
    public override Encoding Encoding => Encoding.Default;

    private void AppendText(string text)
    {
        textBox.AppendText(text);
        textBox.ScrollToCaret();
    }

    public override void Write(string? str) => textBox.Invoke(new AppendTextDelegate(AppendText), str);

    public override void WriteLine(string? value)
    {
        var line = value + Environment.NewLine;
        textBox.Invoke(new AppendTextDelegate(AppendText), line);
    }
}