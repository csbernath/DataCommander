using System.Runtime.Serialization;

namespace Foundation.Deployment.Commands;

[DataContract]
public class DeleteUpdater(string directory) : DeploymentCommand
{
    [DataMember] public string Directory = directory;
}