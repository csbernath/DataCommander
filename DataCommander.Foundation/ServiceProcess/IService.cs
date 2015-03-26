namespace DataCommander.Foundation.ServiceProcess
{
    using System;
    using System.ServiceProcess;

    /// <summary>
    /// 
    /// </summary>
    public interface IService
    {
        /// <summary>
        /// 
        /// </summary>
        bool CanPauseAndContinue
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceBase"></param>
        void Initialize( ServiceBase serviceBase );

        /// <summary>
        /// 
        /// </summary>
        void Start( string[] args );

        /// <summary>
        /// 
        /// </summary>
        void Stop();

        /// <summary>
        /// 
        /// </summary>
        void Pause();

        /// <summary>
        /// 
        /// </summary>
        void Continue();
    }
}