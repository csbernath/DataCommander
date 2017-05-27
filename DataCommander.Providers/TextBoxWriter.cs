namespace DataCommander.Providers
{
    using System;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;

    internal delegate void AppendTextDelegate(string text);

    /// <summary>
    /// Summary description for TextBoxWriter.
    /// </summary>
    public class TextBoxWriter : TextWriter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="textBox"></param>
        public TextBoxWriter(TextBoxBase textBox)
        {
            _textBox = textBox;
        }

        /// <summary>
        /// 
        /// </summary>
        public override Encoding Encoding => null;

        private void AppendText(string text)
        {
            _textBox.AppendText(text);
			_textBox.ScrollToCaret();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        public override void Write(string str)
        {
            _textBox.Invoke(new AppendTextDelegate(AppendText), str);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public override void WriteLine(string value)
        {
            var line = value + Environment.NewLine;
            _textBox.Invoke(new AppendTextDelegate(AppendText), line);
        }

        private readonly TextBoxBase _textBox;
    }
}