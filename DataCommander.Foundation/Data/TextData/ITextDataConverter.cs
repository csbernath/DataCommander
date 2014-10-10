namespace DataCommander.Foundation.Data
{
    using System;

	/// <summary>
    /// 
    /// </summary>
    public interface ITextDataConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        Object FromString(String source, TextDataColumn column);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        String ToString(Object value, TextDataColumn column);
    }
}