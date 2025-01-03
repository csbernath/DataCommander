using System;
using System.Runtime.Serialization;

namespace Foundation.Deployment.Commands;

[DataContract]
public class CheckForUpdates(DateTime when) : DeploymentCommand
{
    [DataMember] public DateTime When = when;
}