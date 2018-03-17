using System.Management;
using Foundation.Diagnostics.Assertions;

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
            Assert.IsNotNull(managementObject);

            _managementObject = managementObject;
        }

        /// <summary>
        /// 
        /// </summary>
        public ManagementJobState JobState => (ManagementJobState)(ushort)_managementObject["JobState"];

        /// <summary>
        /// 
        /// </summary>
        public object PercentComplete => _managementObject["PercentComplete"];
    }
}