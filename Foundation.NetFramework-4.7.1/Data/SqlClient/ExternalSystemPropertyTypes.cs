using System;

namespace Foundation.Data.SqlClient
{
    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum ExternalSystemPropertyTypes
    {
        /// <summary>
        /// 
        /// </summary>
        String = TypeCode.String,

        /// <summary>
        /// 
        /// </summary>
        Int32 = TypeCode.Int32,

        /// <summary>
        /// 
        /// </summary>
        Encrypted = 128
    }
}