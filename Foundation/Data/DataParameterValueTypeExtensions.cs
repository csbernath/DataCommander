namespace Binarit.Foundation.Data
{
    using System;

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
        public static Boolean IsValueOrNull(
#if FOUNDATION_2_0
            DataParameterValueType type
#else
            this DataParameterValueType type
#endif
            )
        {
            return type == DataParameterValueType.Value || type == DataParameterValueType.Null;
        }
    }
}