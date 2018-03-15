namespace Foundation.DomainDrivenDesign
{
    public interface IQueryHandler<in TQuery, out TQueryResult> where TQuery : IQuery<TQueryResult> where TQueryResult : IQueryResult
    {
        TQueryResult Handle(TQuery query);
    }
}