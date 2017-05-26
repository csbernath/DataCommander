namespace Foundation.Data.PTypes
{
    /// <summary>
    /// A Microsoft SQL Server stored procedure parameter can be NULL, DEFAULT, too.
    /// </summary>
    public enum PValueType
    {
        /// <summary>
        /// 
        /// </summary>        
        Default,

        /// <summary>
        /// 
        /// </summary>        
        Empty,

        /// <summary>
        /// 
        /// </summary>
        Null,

        /// <summary>
        /// 
        /// </summary>
        Value
    }
}