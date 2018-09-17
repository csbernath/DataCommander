using System.Threading;
using System.Threading.Tasks;

namespace Foundation.DomainDrivenDesign
{
    public interface IAsyncMessageSender<in TRequest, TResponse> where TRequest : IRequest where TResponse : IResponse
    {
        Task<TResponse> SendAsync(TRequest request, CancellationToken cancellationToken);
    }
}