namespace Foundation.DomainDrivenDesign
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IMessageSender<in TRequest, TResponse> where TRequest : IRequest where TResponse : IResponse
    {
        Task<TResponse> Send(TRequest request, CancellationToken cancellationToken);
    }
}