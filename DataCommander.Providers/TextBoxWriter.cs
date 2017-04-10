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
            this.textBox = textBox;
        }

        /// <summary>
        /// 
        /// </summary>
        public override Encoding Encoding => null;

        private void AppendText(string text)
        {
            this.textBox.AppendText(text);
			this.textBox.ScrollToCaret();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        public override void Write(string str)
        {
            this.textBox.Invoke(new AppendTextDelegate(this.AppendText), str);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public override void WriteLine(string value)
        {
            var line = value + Environment.NewLine;
            this.textBox.Invoke(new AppendTextDelegate(this.AppendText), line);
        }

        private readonly TextBoxBase textBox;
    }
}