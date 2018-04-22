using System;
using System.Windows.Forms;
using Foundation;

namespace DataCommander
{
    public partial class UpdaterForm : Form
    {
        private Updater _updater;

        public UpdaterForm()
        {
            InitializeComponent();
        }

        public Updater Updater => _updater;

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            var eventHandler = new EventHandler(this);
            _updater = new Updater(eventHandler.Handle);
            _updater.Update();
        }

        public void Log(string message)
        {
            richTextBox.AppendText($"[{LocalTime.Default.Now.ToString("HH:mm:ss.fff")}] {message}\r\n");
            Application.DoEvents();
        }
    }
}