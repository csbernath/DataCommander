namespace Foundation.DomainDrivenDesign
{
    public interface IQuery<out TQueryResult> : IRequest where TQueryResult : IQueryResult
    {
    }
}