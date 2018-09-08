namespace DataCommander.Providers
{
    /// <summary>
    /// 
    /// </summary>
    public interface IStandardOutput
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        void WriteLine(params object[] args);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arg"></param>
        void Write(object arg);
    }
}