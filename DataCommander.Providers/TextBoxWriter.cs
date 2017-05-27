using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace DataCommander.Providers
{
    internal delegate void AppendTextDelegate(string text);

    public class TextBoxWriter : TextWriter
    {
        private readonly TextBoxBase _textBox;

        public TextBoxWriter(TextBoxBase textBox)
        {
            _textBox = textBox;
        }

        public override Encoding Encoding => null;

        private void AppendText(string text)
        {
            _textBox.AppendText(text);
            _textBox.ScrollToCaret();
        }

        public override void Write(string str)
        {
            _textBox.Invoke(new AppendTextDelegate(AppendText), str);
        }

        public override void WriteLine(string value)
        {
            var line = value + Environment.NewLine;
            _textBox.Invoke(new AppendTextDelegate(AppendText), line);
        }
    }
}