namespace DataCommander.Foundation.Data
{
    /// <summary>
    /// 
    /// </summary>
    public enum DataParameterValueType
    {
        /// <summary>
        /// 
        /// </summary>
        Default = 0,

        /// <summary>
        /// 
        /// </summary>
        Null,

        /// <summary>
        /// 
        /// </summary>
        Value,

        /// <summary>
        /// 
        /// </summary>
        Void
    }

    /// <summary>
    /// 
    /// </summary>
    public static class DataParameterValueTypeExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsValueOrNull(this DataParameterValueType type)
        {
            return type == DataParameterValueType.Value || type == DataParameterValueType.Null;
        }
    }
}