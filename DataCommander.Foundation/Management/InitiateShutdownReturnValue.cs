namespace DataCommander.Foundation.Management
{
    /// <summary>
    /// 
    /// </summary>
    public enum InitiateShutdownReturnValue
    {
        /// <summary>
        /// 
        /// </summary>
        CompletedWithNoError = 0,

        /// <summary>
        /// 
        /// </summary>
        MethodParametersChecked = 4096,

        /// <summary>
        /// 
        /// </summary>
        Failed = 32768,
                
        /// <summary>
        /// 
        /// </summary>
        AccessDenied = 32769
    }
}