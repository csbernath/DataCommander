namespace Foundation.DomainDrivenDesign
{
    public interface IMessageHandler<in TRequest, out TResponse> where TRequest : IRequest where TResponse : IResponse
    {
        TResponse Handle(TRequest request);
    }
}