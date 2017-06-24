namespace Foundation.DomainDrivenDesign
{
    public interface IOneWayMessageHandler<in TRequest> : IAsyncMessageHandler<TRequest, VoidResponse> where TRequest : IRequest
    {
    }
}