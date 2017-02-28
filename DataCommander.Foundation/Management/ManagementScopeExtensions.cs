namespace DataCommander.Foundation.Management
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Management;
    using DataCommander.Foundation.Linq;

    /// <summary>
    /// 
    /// </summary>
    public static class ManagementScopeExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="managementScope"></param>
        /// <param name="query"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static List<T> ExecuteQuery<T>(this ManagementScope managementScope, string query,
            Func<ManagementObject, T> selector)
        {
            Contract.Requires<ArgumentNullException>(managementScope != null);

            var objectQuery = new ObjectQuery(query);
            List<T> list;

            using (var managementObjectSearcher = new ManagementObjectSearcher(managementScope, objectQuery))
            {
                var managementObjectCollection = managementObjectSearcher.Get();
                var enumerable = managementObjectCollection.Cast<ManagementObject>().Select(selector);
                list = new List<T>(enumerable);
            }

            return list;
        }
    }
}