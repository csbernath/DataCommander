using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Foundation.Log;
using Foundation.Threading.Tasks;

namespace Foundation.DomainDrivenDesign
{
    public class QuerySender
    {
        private readonly Func<string, object> _getHandler;

        public QuerySender(Func<string, object> getHandler)
        {
            _getHandler = getHandler;
        }

        public TQueryResult Send<TQueryResult>(IQuery<TQueryResult> query) where TQueryResult : IQueryResult
        {
            var handler = (IQueryHandler<TQuery, TQueryResult>)_getHandler(query.MessageType);
            return handler.Handle(query);
        }
    }

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