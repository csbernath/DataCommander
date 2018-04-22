using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace DataCommander.Updater
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                Debugger.Launch();
                MessageBox.Show("Updating Data Commander...");

                var applicationExeFileName = args[0];
                var updater = new Updater();
                updater.Update(applicationExeFileName);

                MessageBox.Show("Data Commander updated and started.");

                //Application.EnableVisualStyles();
                //Application.SetCompatibleTextRenderingDefault(false);
                //Application.Run(new UpdaterForm(applicationExeFileName));
            }
            catch (Exception e)
            {
                MessageBox.Show($"Fatal exception:\r\n{e}");
            }
        }
    }
}