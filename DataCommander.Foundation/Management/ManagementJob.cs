using System.Management;

namespace Foundation.Management
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ManagementJob
    {
        private readonly ManagementObject _managementObject;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="managementObject"></param>
        public ManagementJob(ManagementObject managementObject)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(managementObject != null);
#endif

            this._managementObject = managementObject;
        }

        /// <summary>
        /// 
        /// </summary>
        public ManagementJobState JobState => (ManagementJobState)(ushort)this._managementObject["JobState"];

        /// <summary>
        /// 
        /// </summary>
        public object PercentComplete => this._managementObject["PercentComplete"];
    }
}