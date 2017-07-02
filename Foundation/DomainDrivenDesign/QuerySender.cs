using System;

namespace Foundation.DomainDrivenDesign
{
    public class QuerySender
    {
        private readonly Func<Type, object> _getHandler;

        public QuerySender(Func<Type, object> getHandler)
        {
            _getHandler = getHandler;
        }

        public TQueryResult Send<TQueryResult>(IQuery<TQueryResult> query) where TQueryResult : IQueryResult
        {
            var queryType = query.GetType();
            var handler = (Func<TQueryt,TQueryResult>) _getHandler(queryType);
            return (TQueryResult) handler.Handle(query);
        }
    }
}