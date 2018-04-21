using System;
using Foundation.Setup;

namespace Setup
{
    class Program
    {
        static void Main(string[] args)
        {
            var sourceDirectory = Environment.CurrentDirectory;
            var targetDirectory = args[0];
            Installer.Start(sourceDirectory, targetDirectory, "DataCommander.exe");
        }
    }
}