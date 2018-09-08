using System.Threading.Tasks;

namespace Foundation.DomainDrivenDesign
{
    public interface IAsyncMessageHandler<in TRequest, TResponse> where TRequest : IRequest where TResponse : IResponse
    {
        Task<TResponse> HandleAsync(TRequest request);
    }
}