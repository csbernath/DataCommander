namespace DataCommander.Foundation.Management
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Management;
    using System.Text;
    using DataCommander.Foundation.Linq;

    /// <summary>
    /// 
    /// </summary>
    public sealed class MsvmComputerSystem
    {
        private readonly ManagementObject managementObject;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="managementObject"></param>
        public MsvmComputerSystem( ManagementObject managementObject )
        {
            Contract.Requires( managementObject != null );

            this.managementObject = managementObject;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="managementScope"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static MsvmComputerSystem GetByName( ManagementScope managementScope, string name )
        {
            Contract.Requires( managementScope != null );

            string query = string.Format( "SELECT * FROM Msvm_ComputerSystem WHERE Name='{0}'", name );
            List<MsvmComputerSystem> list = managementScope.ExecuteQuery( query, mo => new MsvmComputerSystem( mo ) );
            Contract.Assert( list.Count > 0 );
            MsvmComputerSystem item;

            if (list.Count == 0)
            {
                item = null;
            }
            else
            {
                item = list[ 0 ];
            }

            return item;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="managementScope"></param>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public static List<MsvmComputerSystem> GetByElementName( ManagementScope managementScope, string elementName )
        {
            Contract.Requires( managementScope != null );

            string query = string.Format( "SELECT * FROM Msvm_ComputerSystem WHERE ElementName='{0}'", elementName );
            List<MsvmComputerSystem> list = managementScope.ExecuteQuery( query, mo => new MsvmComputerSystem( mo ) );
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="managementScope"></param>
        /// <param name="elementNames"></param>
        /// <returns></returns>
        public static List<MsvmComputerSystem> GetByElementNames( ManagementScope managementScope, IEnumerable<string> elementNames )
        {
            Contract.Requires( managementScope != null );
            Contract.Requires( elementNames != null );

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat( "SELECT * FROM Msvm_ComputerSystem WHERE" );
            bool first = true;

            foreach (string elementName in elementNames)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    sb.Append( " OR" );
                }

                sb.AppendFormat( " ElementName = '{0}'", elementName );
            }

            string query = sb.ToString();
            List<MsvmComputerSystem> list = managementScope.ExecuteQuery( query, mo => new MsvmComputerSystem( mo ) );
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        public string ElementName
        {
            get
            {
                object elementNameObject = this.managementObject[ "ElementName" ];
                string elementName = (string)elementNameObject;
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
                object enabledStateObject = this.managementObject[ "EnabledState" ];
                UInt16 enabledStateUint16 = (UInt16)enabledStateObject;
                MsvmComputerSystemEnabledState enabledState = (MsvmComputerSystemEnabledState)enabledStateUint16;
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
                object nameObject = this.managementObject[ "Name" ];
                string name = (string)nameObject;
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
                object onTimeObject = this.managementObject[ "OnTimeInMilliseconds" ];
                UInt64 onTimeUInt64 = (UInt64)onTimeObject;
                TimeSpan? onTime;

                if (onTimeUInt64 != 0)
                {
                    onTime = TimeSpan.FromMilliseconds( onTimeUInt64 );
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
        [CLSCompliant( false )]
        public InitiateShutdownReturnValue InitiateShutdown( bool force, string reason )
        {
            string query = string.Format( "SELECT * FROM Msvm_ShutdownComponent WHERE SystemName='{0}'", this.Name );
            ObjectQuery objectQuery = new ObjectQuery( query );
            ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher( this.managementObject.Scope, objectQuery );
            ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();
            ManagementObject shutdownComponent = managementObjectCollection.AsEnumerable<ManagementObject>().First();
            object resultObject = shutdownComponent.InvokeMethod(
                "InitiateShutdown",
                new object[]
                {
                    force,
                    reason
                } );

            UInt32 result = (UInt32)resultObject;
            return (InitiateShutdownReturnValue)result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestedState"></param>
        /// <param name="job"></param>
        /// <returns></returns>
        public MsvmComputerSystemRequestStateChangeReturnValue RequestStateChange( MsvmComputerSystemRequestedState requestedState, out ManagementJob job )
        {
            const string methodName = "RequestStateChange";
            UInt16 requestedStateuInt16 = (UInt16)requestedState;
            ManagementBaseObject inParams = this.managementObject.GetMethodParameters( methodName );
            inParams[ "RequestedState" ] = requestedStateuInt16;
            ManagementBaseObject outParams = this.managementObject.InvokeMethod( methodName, inParams, null );
            MsvmComputerSystemRequestStateChangeReturnValue returnValue = (MsvmComputerSystemRequestStateChangeReturnValue)(UInt32)outParams[ "Returnvalue" ];
            string jobPath = (string)outParams[ "Job" ];

            if (jobPath != null)
            {
                ManagementObject jobObject = new ManagementObject( this.managementObject.Scope, new ManagementPath( jobPath ), null );
                jobObject.Get();
                job = new ManagementJob( jobObject );
            }
            else
            {
                job = null;
            }

            return returnValue;
        }
    }
}