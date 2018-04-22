using System;

namespace DataCommander.Updater
{
    internal sealed class Updater
    {
        public void Update(string applicationExeFileName)
        {
            var updaterDirectory = Environment.CurrentDirectory;
            Foundation.Deployment.Updater.Update(updaterDirectory, applicationExeFileName);
        }
    }
}