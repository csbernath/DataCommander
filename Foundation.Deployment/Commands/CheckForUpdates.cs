using System;
using System.Runtime.Serialization;

namespace Foundation.Deployment.Commands
{
    [DataContract]
    public class CheckForUpdates : DeploymentCommand
    {
        [DataMember] public DateTime When;

        public CheckForUpdates(DateTime @when)
        {
            When = when;
        }
    }
}