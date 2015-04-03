namespace DataCommander.Foundation.Data
{
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
        object FromString(string source, TextDataColumn column);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        string ToString(object value, TextDataColumn column);
    }
}