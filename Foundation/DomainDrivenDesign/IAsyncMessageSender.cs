namespace Foundation.DomainDrivenDesign
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IAsyncMessageSender<in TRequest, TResponse> where TRequest : IRequest where TResponse : IResponse
    {
        Task<TResponse> SendAsync(TRequest request, CancellationToken cancellationToken);
    }
}