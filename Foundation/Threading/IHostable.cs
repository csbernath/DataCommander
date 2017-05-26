namespace DataCommander.Foundation.Threading
{
    using System.ServiceModel;
    using System.ServiceModel.Web;

    /// <summary>
    /// 
    /// </summary>
    [ServiceContractAttribute]
    public interface IHostable
    {
        /// <summary>
        /// 
        /// </summary>
        [WebInvoke( Method = "POST", UriTemplate = "RemotePause" )]
        [OperationContract]
        void RemotePause();

        /// <summary>
        /// 
        /// </summary>
        [WebInvoke( Method = "POST", UriTemplate = "RemoteContinue" )]
        [OperationContract]
        void RemoteContinue();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [WebGet]
        [OperationContract]
        bool GetIsPauseRequested();
    }
}