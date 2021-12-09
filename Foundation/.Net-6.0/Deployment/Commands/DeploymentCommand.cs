using System.Runtime.Serialization;

namespace Foundation.Deployment.Commands
{
    [DataContract]
    [KnownType(typeof(CheckForUpdates))]
    [KnownType(typeof(StartUpdater))]
    [KnownType(typeof(DeleteUpdater))]
    public abstract class DeploymentCommand
    {
    }
}