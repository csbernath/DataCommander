using System;
using System.Windows.Forms;
using Foundation;
using Foundation.Deployment;
using Foundation.Windows.Forms;

namespace DataCommander.Updater
{
    public partial class UpdaterForm : Form
    {
        private Foundation.Deployment.ApplicationStartup _updater;

        public UpdaterForm()
        {
            InitializeComponent();
        }

        public Foundation.Deployment.ApplicationStartup Updater => _updater;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var remoteVersionUri = new Uri("https://raw.githubusercontent.com/csbernath/DataCommander/master/Version.txt");
            var address = "https://github.com/csbernath/DataCommander/releases/download/v{0}/DataCommander.Updater.zip";
            var eventHandler = new EventHandler(this);

            var serializer = new JsonSerializer();
            _updater = new ApplicationStartup(serializer, remoteVersionUri, address, eventHandler.Handle);
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