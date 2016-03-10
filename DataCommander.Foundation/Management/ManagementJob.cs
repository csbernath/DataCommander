namespace DataCommander.Foundation.Management
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Management;

    /// <summary>
    /// 
    /// </summary>
    public sealed class ManagementJob
    {
        private readonly ManagementObject managementObject;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="managementObject"></param>
        public ManagementJob(ManagementObject managementObject)
        {
            Contract.Requires<ArgumentNullException>(managementObject != null);

            this.managementObject = managementObject;
        }

        /// <summary>
        /// 
        /// </summary>
        public ManagementJobState JobState => (ManagementJobState)(ushort)this.managementObject["JobState"];

        /// <summary>
        /// 
        /// </summary>
        public object PercentComplete => this.managementObject["PercentComplete"];
    }
}