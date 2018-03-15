using System;

namespace Foundation.DomainDrivenDesign
{
    public static class QueryHandlerFactory
    {
        public static IAsyncQueryHandler<TQuery, TQueryResult> Create<TQuery, TQueryResult>(Func<TQuery, TQueryResult> handle)
            where TQuery : IQuery<TQueryResult> where TQueryResult : IQueryResult
        {
            return new QueryHandler<TQuery, TQueryResult>(handle);
        }

        public static IAsyncQueryHandler<TQuery, TQueryResult> Create<TQuery, TQueryResult>(IQueryHandler<TQuery, TQueryResult> handler)
            where TQuery : IQuery<TQueryResult> where TQueryResult : IQueryResult
        {
            return new QueryHandler<TQuery, TQueryResult>(handler.Handle);
        }
    }
}