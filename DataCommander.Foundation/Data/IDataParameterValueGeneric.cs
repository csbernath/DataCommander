namespace Foundation.Data
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDataParameterValue<out T> : IDataParameterValue
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