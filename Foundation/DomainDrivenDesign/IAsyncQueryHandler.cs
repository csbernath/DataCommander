namespace Foundation.DomainDrivenDesign
{
    public interface IAsyncQueryHandler<in TQuery, TQueryResult> : IAsyncMessageHandler<TQuery, TQueryResult>
        where TQuery : IQuery where TQueryResult : IQueryResult
    {
    }
}