using System;
using System.Threading.Tasks;

namespace Foundation.DomainDrivenDesign
{
    internal sealed class QueryHandler<TQuery, TQueryResult> : IAsyncQueryHandler<TQuery, TQueryResult>
        where TQuery : IQuery where TQueryResult : IQueryResult
    {
        private readonly Func<TQuery, TQueryResult> _handle;

        public QueryHandler(Func<TQuery, TQueryResult> handle)
        {
            _handle = handle;
        }

        async Task<TQueryResult> IAsyncMessageHandler<TQuery, TQueryResult>.HandleAsync(TQuery query)
        {
            return await Task.Run(() => _handle(query));
        }
    }
}