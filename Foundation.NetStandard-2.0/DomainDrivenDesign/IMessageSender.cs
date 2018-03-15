namespace Foundation.DomainDrivenDesign
{
    public interface IMessageSender<in TRequest, out TResponse> where TRequest : IRequest where TResponse : IResponse
    {
        TResponse Send(TRequest request);
    }
}