using System;
using System.Diagnostics;
using System.Windows.Forms;
using Foundation.Deployment;

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

                var applicationExeFileName = args[0];
                var updaterDirectory = Environment.CurrentDirectory;
                DeploymentHelper.Update(updaterDirectory, applicationExeFileName);

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