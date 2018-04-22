using System.Runtime.Serialization;

namespace Foundation.Deployment
{
    [DataContract]
    public class DeleteUpdater : DeploymentCommand
    {
        [DataMember] public string Directory;
    }
}