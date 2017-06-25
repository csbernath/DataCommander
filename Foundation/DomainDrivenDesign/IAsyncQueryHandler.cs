namespace Foundation.DomainDrivenDesign
{
    public interface IAsyncQueryHandler<TQuery, TQueryResult> : IAsyncMessageHandler<TQuery, TQueryResult>
        where TQuery : IQuery<TQueryResult> where TQueryResult : IQueryResult
    {
    }
}