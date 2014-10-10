namespace DataCommander.Foundation.Data
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDataParameterValue<T> : IDataParameterValue
    {
        /// <summary>
        /// 
        /// </summary>
        T Value
        {
            get;
        }
    }
}