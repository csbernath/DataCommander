using System;
using System.Threading.Tasks;

namespace Foundation.DomainDrivenDesign
{
    public class AsyncQuerySender
    {
        private readonly Func<Type, object> _getHandler;

        public AsyncQuerySender(Func<Type, object> getHandler)
        {
            _getHandler = getHandler;
        }

        public Task<TQueryResult> SendAsync<TQuery, TQueryResult>(TQuery query) where TQuery : IQuery<TQueryResult> where TQueryResult : IQueryResult
        {
            var type = typeof(TQuery);
            var handler = (IAsyncQueryHandler<TQuery, TQueryResult>) _getHandler(type);
            return handler.HandleAsync(query);
        }
    }
}