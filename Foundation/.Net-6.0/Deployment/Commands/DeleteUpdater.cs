using System.Runtime.Serialization;

namespace Foundation.Deployment.Commands;

[DataContract]
public class DeleteUpdater : DeploymentCommand
{
    [DataMember] public string Directory;

    public DeleteUpdater(string directory)
    {
        Directory = directory;
    }
}