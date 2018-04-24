using System;
using System.Windows.Forms;
using Foundation;
using Foundation.Windows.Forms;

namespace DataCommander.Update
{
    public partial class UpdaterForm : Form
    {
        private Updater _updater;

        public UpdaterForm()
        {
            InitializeComponent();
        }

        public Updater Updater => _updater;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var eventHandler = new EventHandler(this);
            _updater = new Updater(eventHandler.Handle);
            _updater.Update().ContinueWith(task =>
            {
                if (task.IsFaulted)
                    MessageBox.Show(task.Exception.ToString());

                this.Invoke(Close);
            });
        }

        public void Log(string message)
        {
            var time = LocalTime.Default.Now.ToString("HH:mm:ss.fff");
            richTextBox.AppendText($"[{time}] {message}\r\n");
        }
    }
}