namespace Foundation.DomainDrivenDesign
{
    public interface IQuery<TQueryResult> : IRequest where TQueryResult : IQueryResult
    {
    }
}