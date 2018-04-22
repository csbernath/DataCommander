using System.Runtime.Serialization;

namespace Foundation.Deployment
{
    [DataContract]
    [KnownType(typeof(CheckForUpdates))]
    [KnownType(typeof(StartUpdater))]
    [KnownType(typeof(DeleteUpdater))]
    public abstract class DeploymentCommand
    {
    }
}