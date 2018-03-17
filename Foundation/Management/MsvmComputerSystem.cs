using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using Foundation.Diagnostics;
using Foundation.Diagnostics.Assertions;
using Foundation.Diagnostics.Contracts;
using Foundation.Linq;

namespace Foundation.Management
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class MsvmComputerSystem
    {
        private readonly ManagementObject _managementObject;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="managementObject"></param>
        public MsvmComputerSystem(ManagementObject managementObject)
        {
            Assert.IsNotNull(managementObject);

            _managementObject = managementObject;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="managementScope"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static MsvmComputerSystem GetByName(ManagementScope managementScope, string name)
        {
            Assert.IsNotNull(managementScope);

            var query = $"SELECT * FROM Msvm_ComputerSystem WHERE Name='{name}'";
            var list = managementScope.ExecuteQuery(query, mo => new MsvmComputerSystem(mo));

            FoundationContract.Assert(list.Count > 0);

            MsvmComputerSystem item;

            if (list.Count == 0)
            {
                item = null;
            }
            else
            {
                item = list[0];
            }

            return item;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="managementScope"></param>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public static List<MsvmComputerSystem> GetByElementName(ManagementScope managementScope, string elementName)
        {
            FoundationContract.Requires<ArgumentException>(managementScope != null);

            var query = $"SELECT * FROM Msvm_ComputerSystem WHERE ElementName='{elementName}'";
            var list = managementScope.ExecuteQuery(query, mo => new MsvmComputerSystem(mo));
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="managementScope"></param>
        /// <param name="elementNames"></param>
        /// <returns></returns>
        public static List<MsvmComputerSystem> GetByElementNames(ManagementScope managementScope,
            IEnumerable<string> elementNames)
        {
            Assert.IsNotNull(managementScope);
            Assert.IsNotNull(elementNames);

            var sb = new StringBuilder();
            sb.AppendFormat("SELECT * FROM Msvm_ComputerSystem WHERE");
            var first = true;

            foreach (var elementName in elementNames)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    sb.Append(" OR");
                }

                sb.AppendFormat(" ElementName = '{0}'", elementName);
            }

            var query = sb.ToString();
            var list = managementScope.ExecuteQuery(query, mo => new MsvmComputerSystem(mo));
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        public string ElementName
        {
            get
            {
                var elementNameObject = _managementObject["ElementName"];
                var elementName = (string) elementNameObject;
                return elementName;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public MsvmComputerSystemEnabledState EnabledState
        {
            get
            {
                var enabledStateObject = _managementObject["EnabledState"];
                var enabledStateUint16 = (ushort) enabledStateObject;
                var enabledState = (MsvmComputerSystemEnabledState) enabledStateUint16;
                return enabledState;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get
            {
                var nameObject = _managementObject["Name"];
                var name = (string) nameObject;
                return name;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public TimeSpan? OnTime
        {
            get
            {
                var onTimeObject = _managementObject["OnTimeInMilliseconds"];
                var onTimeUInt64 = (ulong) onTimeObject;
                TimeSpan? onTime;

                if (onTimeUInt64 != 0)
                {
                    onTime = TimeSpan.FromMilliseconds(onTimeUInt64);
                }
                else
                {
                    onTime = null;
                }

                return onTime;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="force"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        [CLSCompliant(false)]
        public InitiateShutdownReturnValue InitiateShutdown(bool force, string reason)
        {
            var query = $"SELECT * FROM Msvm_ShutdownComponent WHERE SystemName='{Name}'";
            var objectQuery = new ObjectQuery(query);
            var managementObjectSearcher = new ManagementObjectSearcher(
                _managementObject.Scope, objectQuery);
            var managementObjectCollection = managementObjectSearcher.Get();
            var shutdownComponent = managementObjectCollection.Cast<ManagementObject>().First();
            var resultObject = shutdownComponent.InvokeMethod(
                "InitiateShutdown",
                new object[]
                {
                    force,
                    reason
                });

            var result = (uint) resultObject;
            return (InitiateShutdownReturnValue) result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestedState"></param>
        /// <param name="job"></param>
        /// <returns></returns>
        public MsvmComputerSystemRequestStateChangeReturnValue RequestStateChange(
            MsvmComputerSystemRequestedState requestedState, out ManagementJob job)
        {
            const string methodName = "RequestStateChange";
            var requestedStateuInt16 = (ushort) requestedState;
            var inParams = _managementObject.GetMethodParameters(methodName);
            inParams["RequestedState"] = requestedStateuInt16;
            var outParams = _managementObject.InvokeMethod(methodName, inParams, null);
            var returnValue = (MsvmComputerSystemRequestStateChangeReturnValue) (uint) outParams["Returnvalue"];
            var jobPath = (string) outParams["Job"];

            if (jobPath != null)
            {
                var jobObject = new ManagementObject(_managementObject.Scope,
                    new ManagementPath(jobPath), null);
                jobObject.Get();
                job = new ManagementJob(jobObject);
            }
            else
            {
                job = null;
            }

            return returnValue;
        }
    }
}