using System.Diagnostics;
using System.IO;
using Foundation.Deployment.Commands;

namespace Foundation.Deployment
{
    public static class UpdaterStartup
    {
        public static void Update(string applicationName, string updaterDirectory, string applicationExeFileName)
        {
            var applicationDirectory = Path.GetDirectoryName(applicationExeFileName);


            var sourceDirectory = Path.Combine(updaterDirectory, applicationName);

            Directory.Move(sourceDirectory, applicationDirectory);



            DeploymentCommandRepository.Save(applicationName, new DeleteUpdater {Directory = updaterDirectory});

            var processStartInfo = new ProcessStartInfo();
            processStartInfo.WorkingDirectory = applicationDirectory;
            processStartInfo.FileName = applicationExeFileName;
            processStartInfo.Arguments = applicationDirectory;
            Process.Start(processStartInfo);
        }

        //private static void Copy
    }
}