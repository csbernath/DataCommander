namespace DataCommander.Foundation.Management
{
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
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(managementObject != null);
#endif

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